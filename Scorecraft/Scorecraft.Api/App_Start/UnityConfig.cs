using System;
using System.Data.Entity;
using Unity;
using Scorecraft.Data;
using Scorecraft.Sofa;
using Scorecraft.Sofa.Models;
using Scorecraft.Sofa.Scrapers;

namespace Scorecraft.Api
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your type's mappings here.
            container.RegisterSingleton<ISofaLogger, SofaLogger>();
            container.RegisterSingleton<DbContext, SCDbContext>();

            container.RegisterType(typeof(IRepository<>), typeof(Repository<>));

            container.RegisterType(typeof(ISofaScraper<,>), typeof(SofaScraper<,>));
            container.RegisterType(typeof(ISofaScraper<SofaRegion, RegionInfo>), typeof(RegionScraper));
            container.RegisterType(typeof(ISofaScraper<SofaCompetition, CompetitionInfo>), typeof(CompetitionScraper));
            container.RegisterType(typeof(ISofaScraper<SofaSeason,SeasonInfo>), typeof(SeasonScraper));
            container.RegisterType(typeof(ISofaScraper<SofaRound,RoundInfo>), typeof(RoundScraper));
            container.RegisterType(typeof(ISofaScraper<SofaGroup,GroupInfo>), typeof(GroupScraper));
        }
    }
}