﻿using PearAdmin.AbpTemplate.EntityFrameworkCore.Seed.Common;

namespace PearAdmin.AbpTemplate.EntityFrameworkCore.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly AbpTemplateDbContext _context;

        public InitialHostDbBuilder(AbpTemplateDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultEditionCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();

            _context.SaveChanges();
        }
    }
}
