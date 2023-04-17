using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using VmarmyshApp.EF;
using VmarmyshApp.Models;
using VmarmyshApp.Models.Enums;
using VmarmyshApp.Models.ExceptionsModels;

namespace VmarmyshApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BinaryTreeNodesController : ControllerBase
    {
        private readonly BinaryTreeNodeContext _context;
        private readonly ILogger _logger;

        public BinaryTreeNodesController(BinaryTreeNodeContext context, ILogger<BinaryTreeNodesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/BinaryTreeNodes
        [HttpGet]
        public JsonResult GetNodes()
        {
            try
            {
                var nodeList = _context.Nodes.ToList();

                _logger.LogInformation($"Node list created: {nodeList}");

                return new JsonResult(nodeList) { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                var e = new SecureException(ex);
                return new JsonResult(e) { StatusCode = 500 };
            }
        }

        // GET: api/BinaryTreeNodes/5
        [HttpGet("{id}")]
        public JsonResult GetBinaryTreeNode(Guid id)
        {
            try
            {
                var binaryTreeNode = _context.Nodes.Find(id);

                _logger.LogInformation($"BinaryTreeNode found: {binaryTreeNode}");

                return new JsonResult(binaryTreeNode) { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                var e = new SecureException(ex);
                return new JsonResult(e) { StatusCode = 500 };
            }
        }

        // PUT: api/BinaryTreeNodes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public JsonResult PutBinaryTreeNode(Guid id, BinaryTreeNode binaryTreeNode)
        {
            try
            {
                CheckPutNode(id, binaryTreeNode);

                var editNode = _context.Nodes.Find(id);
                editNode.Name = binaryTreeNode.Name;
                editNode.Side = binaryTreeNode.Side;
                
                _context.SaveChanges();

                _logger.LogInformation($"Edited node: {editNode}");

                return new JsonResult(editNode) { StatusCode = 204 };
            }
            catch(SecureException ex)
            {
                _logger.LogError(ex.ToString());
                return new JsonResult(ex) { StatusCode = 500 };
            }
            catch (Exception ex)
            {
                var e = new SecureException(ex);
                _logger.LogError(e.ToString());
                return new JsonResult(e) { StatusCode = 500 };
            }
        }

        // POST: api/BinaryTreeNodes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public JsonResult PostBinaryTreeNode(BinaryTreeNode newBinaryTreeNode)
        {
            try
            {
                //add root node
                if (newBinaryTreeNode.ParentNodeId == null)
                {
                    CheckAddRootNode(newBinaryTreeNode);

                    _context.Nodes.Add(newBinaryTreeNode);
                    _context.SaveChanges();

                    _logger.LogInformation($"Added new tree, root node: {newBinaryTreeNode}");

                    return new JsonResult(newBinaryTreeNode) { StatusCode = 201 };
                }
                //add new node
                else
                {
                    CheckAddNewNode(newBinaryTreeNode);

                    var parent = _context.Nodes?.FirstOrDefault(node => node.Id == newBinaryTreeNode.ParentNodeId);

                    switch (newBinaryTreeNode.Side)
                    {
                        case NodeSide.Left:
                            parent.LeftChild = newBinaryTreeNode;
                            break;
                        case NodeSide.Right:
                            parent.RightChild = newBinaryTreeNode;
                            break;
                    }

                    _context.Nodes.Add(newBinaryTreeNode);
                    _context.SaveChanges();
                    _logger.LogInformation($"Added new node: {newBinaryTreeNode}");

                    return new JsonResult(newBinaryTreeNode) { StatusCode = 201 };
                }
            }
            catch (SecureException ex)
            {
                _logger.LogError(ex.ToString());
                return new JsonResult(ex) { StatusCode = 500 };
            }
            catch (Exception ex)
            {
                var e = new SecureException(ex);
                _logger.LogError(e.ToString());
                return new JsonResult(e) { StatusCode = 500 };
            }
        }

        // DELETE: api/BinaryTreeNodes/5
        [HttpDelete("{id}")]
        public JsonResult DeleteBinaryTreeNode(Guid id)
        {
            try
            {
                var nodeToDelte = CheckDeleteNode(id);

                //delete tree (root element)
                if (nodeToDelte.ParentNodeId == null)
                {
                    _context.Nodes.Remove(nodeToDelte);
                }
                //delete node
                else
                {
                    var parentNode = _context.Nodes.Find(nodeToDelte.ParentNodeId);
                    parentNode.LeftChild = null;
                    parentNode.RightChild = null;

                    _context.Nodes.Remove(nodeToDelte);
                }

                _context.SaveChanges();

                _logger.LogInformation($"Deleted: {nodeToDelte}");

                return new JsonResult(nodeToDelte) { StatusCode = 204 };
            }
            catch (SecureException ex)
            {
                _logger.LogError(ex.ToString());
                return new JsonResult(ex) { StatusCode = 500 };
            }
            catch (Exception ex)
            {
                var e = new SecureException(ex);
                _logger.LogError(e.ToString());
                return new JsonResult(e) { StatusCode = 500 };
            }
        }

        private void CheckAddRootNode(BinaryTreeNode rootNode)
        {
            if (rootNode.ParentNodeId != null)
            {
                throw new SecureException("Root node Parent must be null");
            }
            if (rootNode.LeftChildId != null || rootNode.RightChildId != null)
            {
                throw new SecureException("Root node Children must be null");
            }
            if (rootNode.Side != null)
            {
                throw new SecureException("Root node Side must be null");
            }
        }

        private void CheckAddNewNode(BinaryTreeNode newNode)
        {
            if (newNode.ParentNodeId == null)
                throw new SecureException("New node must have Parent");

            if (_context.Nodes?.FirstOrDefault(node => node.Id == newNode.ParentNodeId) == null)
                throw new SecureException($"Parent with id {newNode.ParentNodeId} doesnt exists");

            if (newNode.LeftChildId != null || newNode.RightChildId != null)
                throw new SecureException("New node Children must be null");

            if (newNode.Side == null)
                throw new SecureException("New node must have Side");
        }
        
        private void CheckPutNode(Guid id, BinaryTreeNode binaryTreeNode)
        {
            var node = _context.Nodes.Find(id);

            if (node == null || node.Id != binaryTreeNode.Id)
            {
                throw new SecureException("Node to edit not found");
            }
            if (node.ParentNodeId != binaryTreeNode.ParentNodeId)
            {
                throw new SecureException("Parents of nodes must be the same");
            }
            if (node.LeftChildId != binaryTreeNode.LeftChildId && node.RightChildId != binaryTreeNode.RightChildId)
            {
                throw new SecureException("Node children must be the same");
            }
            if ((node.Side != binaryTreeNode.Side && node.HasChildren) || node.ParentNodeId == null)
            {
                throw new SecureException("Switching sides is only available if the node has no children and is not the root element");
            }
        }

        private BinaryTreeNode CheckDeleteNode(Guid id)
        {
            var nodeToDelete = _context.Nodes.Find(id);

            if (nodeToDelete == null)
            {
                throw new SecureException("Node to delete not found");
            }
            if (nodeToDelete.HasChildren)
            {
                throw new SecureException("You have to delete all children nodes first");
            }

            return nodeToDelete;
        }
    }
}
