using System.Drawing;
using System.Linq;
using System.Reflection;

namespace Casey.Utility
{
    public class ResourceManager
    {
        private Assembly _assembly;

        public ResourceManager()
        {
            _assembly = Assembly.GetCallingAssembly();
        }

        public Image GetImageBySuffix(string suffix)
        {
            var fullName = _assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith(suffix));
            if (!string.IsNullOrEmpty(fullName))
            {
                using (var stream = _assembly.GetManifestResourceStream(fullName))
                {
                    if (stream != null)
                    {
                        return Image.FromStream(stream);
                    }
                }
            }

            return null;
        }
    }
}
