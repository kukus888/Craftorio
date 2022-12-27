using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Net.Http;
using System.Net.Http.Json;
using System.IO;

namespace Craftorio.Shared
{
    public interface IPlayerController
    {
        /// <summary>
        /// Gets player's available money
        /// </summary>
        /// <returns></returns>
        int GetMoney(string username);
        /// <summary>
        /// Registers a player, loading its state etc
        /// </summary>
        void RegisterPlayer(Player player);
        Player GetPlayer(string username);
    }
    public class PlayerController : IPlayerController
    {
        protected PlayerController Instance { get; }
        private List<Player> playerList { get; }
        public PlayerController()
        {
            if (Instance == null)
            {
                Instance = this;
                playerList = new List<Player>();
            }
            else
            {
                //Singleton has already been created
            }
        }
        public void RegisterPlayer(Player player)
        {
            playerList.Add(player);
        }
        public Player GetPlayer(string username)
        {
            return playerList.Find(x => x.username == username);
        }
        public int GetMoney(string username)
        {
            return playerList.Find(x => x.username == username).resourceManager.GetAmountOfResource("Money");
        }
    }
    public class Player
    {
        public ResourceManager resourceManager;
        public ResourceFactoryManager resourceFactoryManager;
        public string username;
        public Player()
        {
            this.resourceManager = new ResourceManager();
            this.resourceFactoryManager = new ResourceFactoryManager();
            this.username = null;
        }
        public Player(string username)
        {
            this.resourceManager = new ResourceManager();
            this.resourceFactoryManager = new ResourceFactoryManager();
            resourceManager.RegisterResource("Money");
            this.username = username;
            CreatePlayer();
        }
        public void CreatePlayer()
        {
            //player is new
            resourceManager.AddResource("Money", 100);
            //Load ResourceFactories
            resourceFactoryManager.resourceFactories.Add(new ResourceFactory("Forest", 20, 1, 2));
            resourceFactoryManager.resourceFactories.Add(new ResourceFactory("Sawmill", 100, 0, 10));
            resourceFactoryManager.resourceFactories.Add(new ResourceFactory("Paper mill", 250, 0, 20));
            resourceFactoryManager.resourceFactories.Add(new ResourceFactory("Furniture store", 1000, 0, 85));
        }
        public int GetMoney()
        {
            return resourceManager.GetAmountOfResource("Money");
        }
    }
    public class ResourceManager
    {
        public List<Resource> resources;
        public ResourceManager()
        {
            resources = new List<Resource>();
        }
        public int Tick()
        {
            int revenue = 0;
            /*foreach(ResourceFactory resource in resources)
            {
                revenue += resource.GeneratingPerTick;
            }*/
            return revenue;
        }
        /// <summary>
        /// Gets a list of resources and their amounts
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<string,int>> GetResourceAmountPairs()
        {
            List<KeyValuePair<string,int>> keyValuePairs= new List<KeyValuePair<string,int>>();
            if(resources == null || resources.Count == 0) { return keyValuePairs; }
            foreach(Resource resource in resources)
            {
                keyValuePairs.Add(new KeyValuePair<string,int>(resource.ResourceName, resource.Amount));
            }
            return keyValuePairs;
        }
        public void RegisterResource(string resourceName)
        {
            Resource r = new Resource();
            r.ResourceName = resourceName;
            r.Amount = 0;
            resources.Add(r);
        }
        public void AddResource(string resourceName, int amount)
        {
            try
            {   
                resources.Find(x => x.ResourceName == resourceName).Amount = amount;
            }
            catch (ArgumentNullException ex)
            {
                throw new UnknownResourceException($"Resource {resourceName} has not been registered.");
            }
        }
        public void ChangeResourceAmount(string resourceName, MathOperation operation, int amount)
        {
            switch (operation)
            {
                case MathOperation.Add:
                    resources.Find(x => x.ResourceName == resourceName).Amount += amount;
                    break;
                case MathOperation.Subtract:
                    resources.Find(x => x.ResourceName == resourceName).Amount -= amount;
                    break;
                case MathOperation.Multiply:
                    resources.Find(x => x.ResourceName == resourceName).Amount *= amount;
                    break;
                case MathOperation.Divide:
                    resources.Find(x => x.ResourceName == resourceName).Amount /= amount;
                    break;
            }
        }
        public int GetAmountOfResource(string resourceName)
        {
            return resources.Find(x => x.ResourceName == resourceName).Amount;
        }
        class UnknownResourceException : Exception
        {
            public UnknownResourceException(string message) { }
        }
    }
    public enum MathOperation
    {
        Add, Subtract, Multiply, Divide
    }
    public class Resource
    {
        /// <summary>
        /// What's this upgrades name
        /// </summary>
        public string ResourceName { get; set; } = "";
        /// <summary>
        /// How many upgrades has player bought
        /// </summary>
        public int Amount { get; set; } = 0;
        public Resource()
        {

        }
    }
    public class ResourceFactoryManager
    {
        public List<ResourceFactory> resourceFactories;
        public ResourceFactoryManager()
        {
            resourceFactories = new List<ResourceFactory>();
        }
        public void RegisterResourceFactory(ResourceFactory r)
        {
            resourceFactories.Add(r);
        }
        public void RecalculateAll()
        {
            foreach(ResourceFactory r in resourceFactories)
            {
                r.RecalculateGeneratingPerTick();
            }
        }
    }
    public class ResourceFactory
    {
        /// <summary>
        /// What's this upgrades name
        /// </summary>
        public string resourceName { get; set; } = "";
        /// <summary>
        /// How much the upgrade costs
        /// </summary>
        public int upgradeCost { get; set; } = 0;
        /// <summary>
        /// How many upgrades has player bought
        /// </summary>
        public int amountBought { get; set; } = 0;
        /// <summary>
        /// Amount which this upgrade will generate each tick, for one upgrade
        /// </summary>
        public int oneGeneratesPerTick { get; set; } = 0;
        /// <summary>
        /// How much this upgrade generates per tick, including multiples of this upgrade.
        /// </summary>
        public int generatingPerTick { get; set; } = 0;
        public ResourceFactory(string _resourceName, int _upgradeCost, int _amountBought, int _oneGeneratesPerTick)
        {
            this.resourceName = _resourceName;
            this.upgradeCost = _upgradeCost;
            this.amountBought = _amountBought;
            this.oneGeneratesPerTick = _oneGeneratesPerTick;
            RecalculateGeneratingPerTick();
        }
        public ResourceFactory()
        {
            this.resourceName = null;
            this.upgradeCost = 0;
            this.amountBought = 0;
            this.oneGeneratesPerTick = 0;
        }
        public void RecalculateGeneratingPerTick()
        {
            this.generatingPerTick = amountBought * oneGeneratesPerTick;
        }
        public void RaiseUpgradePrice(double upgradeCoefficient)
        {
            this.upgradeCost = (int)(upgradeCoefficient * this.upgradeCost);
        }
    }
}
