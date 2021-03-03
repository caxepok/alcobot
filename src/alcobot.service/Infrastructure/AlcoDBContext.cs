using alcobot.service.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace alcobot.service.Infrastructure
{
    public class AlcoDBContext : DbContext
    {
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Drinker> Drinkers { get; set; }
        public DbSet<Drink> Drinks { get; set; }
        public DbSet<VolumeRegex> VolumeRegexes { get; set; }
        public DbSet<Alcohole> Alcoholes { get; set; }
        public DbSet<Message> Messages { get; set; }

        public AlcoDBContext(DbContextOptions<AlcoDBContext> options) : base(options)
        {
        }
    }
}
