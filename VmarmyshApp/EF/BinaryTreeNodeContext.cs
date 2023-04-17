using Microsoft.EntityFrameworkCore;
using VmarmyshApp.Models;

namespace VmarmyshApp.EF
{
    public class BinaryTreeNodeContext : DbContext
    {
        public DbSet<BinaryTreeNode> Nodes { get; set; }

        public BinaryTreeNodeContext()
        {
        }

        public BinaryTreeNodeContext(DbContextOptions<BinaryTreeNodeContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BinaryTreeNode>()
                .HasOne(n => n.LeftChild)
                .WithMany()
                .HasForeignKey(n => n.LeftChildId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BinaryTreeNode>()
                .HasOne(n => n.RightChild)
                .WithMany()
                .HasForeignKey(n => n.RightChildId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BinaryTreeNode>()
                .HasOne(n => n.ParentNode)
                .WithMany()
                .HasForeignKey(n => n.ParentNodeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}