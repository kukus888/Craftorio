using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Craftorio.Shared
{
    public class sPlayer
    {
        public sPlayer Instance;
        public sResourceManager resourceManager;
        public string username;
        public sPlayer(string username)
        {
            if(this.Instance == null)
            {
                this.resourceManager = new sResourceManager();
                resourceManager.RegisterResource("Money");
                this.username = username;
                this.Instance = this;
                CreateOrLoadPlayer();
            }
        }
        public void Tick()
        {
            //Money+=resourceManager.Tick();
        }
        public void CreateOrLoadPlayer()
        {
            this.Instance.resourceManager.AddResource("Money", 100);
        }
    }
    public class sResourceManager
    {
        private List<Resource> resources;
        public sResourceManager Instance;
        public sResourceManager()
        {
            if(Instance == null)
            {
                resources = new List<Resource>();
                Instance = this;
            }
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
            foreach(Resource resource in Instance.resources)
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
            Instance.resources.Add(r);
        }
        public void AddResource(string resourceName, int amount)
        {
            try
            {   
                Instance.resources.Find(x => x.ResourceName == resourceName).Amount = amount;
            }
            catch (ArgumentNullException ex)
            {
                throw new UnknownResourceException($"Resource {resourceName} has not been registered.");
            }
        }
        class UnknownResourceException : Exception
        {
            public UnknownResourceException(string message) { }
        }
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
    public class ResourceFactory
    {
        /// <summary>
        /// What's this upgrades name
        /// </summary>
        public string ResourceName { get; set; } = "";
        /// <summary>
        /// How much the upgrade costs
        /// </summary>
        public int UpgradeCost { get; set; } = 0;
        /// <summary>
        /// How many upgrades has player bought
        /// </summary>
        public int AmountBought { get; set; } = 0;
        /// <summary>
        /// Amount which this upgrade will generate each tick, for one upgrade
        /// </summary>
        public int OneGeneratesPerTick { get; set; } = 0;
        /// <summary>
        /// How much this upgrade generates per tick, including multiples of this upgrade.
        /// </summary>
        public int GeneratingPerTick { get; set; } = 0;
        public ResourceFactory()
        {
            throw new NotImplementedException();
        }
        public void RecalculateGeneratingPerTick()
        {
            this.GeneratingPerTick = AmountBought * OneGeneratesPerTick;
        }
        public void TryBuyUpgrade()
        {
            sPlayer player = new sPlayer("");
            /*if (player.Money >= UpgradeCost)
            {
                AmountBought++;
                player.Money -= UpgradeCost;
            }*/
            RecalculateGeneratingPerTick();
        }
    }
}
