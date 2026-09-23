using Keeltekooli.Controllers;
using Keeltekooli.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using TARpv24Keeltekooli.XUnitTesting.Mock;
using ZendeskApi_v2.Requests;

namespace TARpv24Keeltekooli.XUnitTesting
{
    public abstract class TestBase
    {
        protected IServiceProvider serviceProvider { get; set; }
        protected TestBase()
        {
            var services = new ServiceCollection();
            SetupServices(services);
            serviceProvider = services.BuildServiceProvider();
        }
        /// <summary>
        /// Seame üles testide läbiviimiseks vajalikud kontrollerid mujalt projektist
        /// See meetod annab ka mälusoleva andmebaasi, mida testideks kasutada,
        /// VIPER-tüüpi projektis toimib kui "program.cs" analoog, ent lühidal kujul.
        /// </summary>
        /// <param name="services"></param>
        private void SetupServices(ServiceCollection services)
        {
            //services.AddScoped<IRegistreeriminesServices, RegistreeriminesServices>();
            services.AddScoped<KoolitusController>();
            services.AddScoped<IHostEnvironment, MockIHostEnvironment>();

            services.AddDbContext<ApplicationUser>
                (x =>
                {
                    x.UseInMemoryDatabase("TEST");
                    x.ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                });
            RegisterMacros(services);
        }
        public void Dispose()
        {

        }
        /// <summary>
        /// Leia üles kindel teenus, teenusepakkujalt.
        /// serviceProvider omab kontrollerise instantse, ning GetService hangib selle
        /// X tüüpi kontrolleri
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T Svc<T>()
        {
            return serviceProvider.GetService<T>();
        }
        /// <summary>
        /// registreerib macrodes teenusied kui nad ei ole liidesed ja ei ole abstraktsed
        /// on vaja test setupide seadistuseks
        /// Makro -→ Teenus
        /// </summary>
        /// <param name="services">Teenused, siia lisatakse makrodest muid teenuseid </param>
        private void RegisterMacros(ServiceCollection services)
        {
            var macroBaseType = typeof(IMacros);
            var macros = macroBaseType.Assembly.GetTypes()
                .Where(t => t.IsInterface && !t.IsAbstract);
            foreach(var macro in macros)
            {
                services.AddSingleton(macro);
            }
        }
    }
}
