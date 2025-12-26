using System.Collections.Generic;
using System.Text;

namespace HealthTech.Services
{
    // 1. Component (Abstract Base)
    public abstract class MedicalComponent
    {
        protected string name;
        
        public MedicalComponent(string name) 
        { 
            this.name = name; 
        }

        public abstract string GetHtmlSummary(int indentLevel = 0);
    }

    // 2. Leaf (Individual Record)
    public class SingleRecord : MedicalComponent
    {
        public SingleRecord(string name) : base(name) { }

        public override string GetHtmlSummary(int indentLevel)
        {
            return $"<div style='margin-left:{indentLevel * 20}px'>📄 {name}</div>";
        }
    }

    // 3. Composite (Folder of Records)
    public class RecordFolder : MedicalComponent
    {
        // Initialize the list immediately to avoid null errors
        private List<MedicalComponent> _children = new List<MedicalComponent>();

        public RecordFolder(string name) : base(name) { }

        // Standard method syntax (Safe from syntax errors)
        public void Add(MedicalComponent component)
        {
            _children.Add(component);
        }

        public void Remove(MedicalComponent component)
        {
            _children.Remove(component);
        }

        public override string GetHtmlSummary(int indentLevel)
        {
            StringBuilder sb = new StringBuilder();
            
            // Render the folder name
            sb.Append($"<div style='margin-left:{indentLevel * 20}px; font-weight:bold;'>📂 {name}</div>");

            // Recursively print children
            foreach (var child in _children)
            {
                sb.Append(child.GetHtmlSummary(indentLevel + 1));
            }
            
            return sb.ToString();
        }
    }
}