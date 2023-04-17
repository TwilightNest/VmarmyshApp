using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using VmarmyshApp.Models.Enums;

namespace VmarmyshApp.Models
{
    public class BinaryTreeNode
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [ForeignKey(nameof(LeftChild))]
        public Guid? LeftChildId { get; set; }

        [ForeignKey(nameof(RightChild))]
        public Guid? RightChildId { get; set; }

        [ForeignKey(nameof(ParentNode))]
        public Guid? ParentNodeId { get; set; }

        [ForeignKey(nameof(LeftChildId))]
        public BinaryTreeNode? LeftChild { get; set; }

        [ForeignKey(nameof(RightChildId))]
        public BinaryTreeNode? RightChild { get; set; }

        [ForeignKey(nameof(ParentNodeId))]
        public BinaryTreeNode? ParentNode { get; set; }

        public NodeSide? Side { get; set; }

        [NotMapped]
        public bool HasChildren => (LeftChildId.HasValue || RightChildId.HasValue);

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}