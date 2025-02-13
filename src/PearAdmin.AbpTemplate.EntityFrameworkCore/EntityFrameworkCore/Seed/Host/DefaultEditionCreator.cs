﻿using System.Linq;
using Abp.Application.Editions;
using Abp.Application.Features;
using Microsoft.EntityFrameworkCore;
using PearAdmin.AbpTemplate.Editions;

namespace PearAdmin.AbpTemplate.EntityFrameworkCore.Seed.Host
{
    public class DefaultEditionCreator
    {
        private readonly AbpTemplateDbContext _context;

        public DefaultEditionCreator(AbpTemplateDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateEditions();
        }

        private void CreateEditions()
        {
            // 初始化默认版本
            var defaultEdition = _context.Editions.IgnoreQueryFilters().FirstOrDefault(e => e.Name == EditionManager.DefaultEditionName);
            if (defaultEdition == null)
            {
                defaultEdition = new Edition()
                {
                    Name = EditionManager.DefaultEditionName,
                    DisplayName = EditionManager.DefaultEditionName
                };
                _context.Editions.Add(defaultEdition);
                _context.SaveChanges();

                // 为当前默认版本添加初始功能
            }
        }

        private void CreateFeatureIfNotExists(int editionId, string featureName, bool isEnabled)
        {
            if (_context.EditionFeatureSettings.IgnoreQueryFilters().Any(ef => ef.EditionId == editionId && ef.Name == featureName))
            {
                return;
            }

            _context.EditionFeatureSettings.Add(new EditionFeatureSetting
            {
                Name = featureName,
                Value = isEnabled.ToString(),
                EditionId = editionId
            });
            _context.SaveChanges();
        }
    }
}
