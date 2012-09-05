using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mogre;

namespace Mogre.Demo.MogreForm
{
    public partial class MogreForm : Form
    {
        protected OgreWindow mogreWin;

        public MogreForm()
        {
            InitializeComponent();
            this.Disposed += new EventHandler(MogreForm_Disposed);

            mogreWin = new OgreWindow(new Point(100, 30), mogrePanel.Handle);
            mogreWin.InitMogre();
        }

        private void MogreForm_Paint(object sender, PaintEventArgs e)
        {
            mogreWin.Paint();
        }

        void MogreForm_Disposed(object sender, EventArgs e)
        {
            mogreWin.Dispose();
        }
    }

    public class OgreWindow
    {
        public Root root;
        public SceneManager sceneMgr;
        
        protected Camera camera;
        protected Viewport viewport;
        protected RenderWindow window;
        protected Point position;
        protected IntPtr hWnd;

        public OgreWindow(Point origin, IntPtr hWnd)
        {
            position = origin;
            this.hWnd = hWnd;
        }

        public void InitMogre()
        {

            //----------------------------------------------------- 
            // 1 enter ogre 
            //----------------------------------------------------- 
            root = new Root();

            //----------------------------------------------------- 
            // 2 configure resource paths
            //----------------------------------------------------- 
            ConfigFile cf = new ConfigFile();
            cf.Load("resources.cfg", "\t:=", true);

            // Go through all sections & settings in the file
            ConfigFile.SectionIterator seci = cf.GetSectionIterator();

            String secName, typeName, archName;

            // Normally we would use the foreach syntax, which enumerates the values, but in this case we need CurrentKey too;
            while (seci.MoveNext())
            {
                secName = seci.CurrentKey;
                ConfigFile.SettingsMultiMap settings = seci.Current;
                foreach (KeyValuePair<string, string> pair in settings)
                {
                    typeName = pair.Key;
                    archName = pair.Value;
                    ResourceGroupManager.Singleton.AddResourceLocation(archName, typeName, secName);
                }
            }

            //----------------------------------------------------- 
            // 3 Configures the application and creates the window
            //----------------------------------------------------- 
            bool foundit = false;
            foreach (RenderSystem rs in root.GetAvailableRenderers())
            {
                root.RenderSystem = rs;
                String rname = root.RenderSystem.Name;
                if (rname == "Direct3D9 Rendering Subsystem")
                {
                    foundit = true;
                    break;
                }
            }

            if (!foundit)
                return; //we didn't find it... Raise exception?

            //we found it, we might as well use it!
            root.RenderSystem.SetConfigOption("Full Screen", "No");
            root.RenderSystem.SetConfigOption("Video Mode", "640 x 480 @ 32-bit colour");

            root.Initialise(false);
	        NameValuePairList misc = new NameValuePairList();
            misc["externalWindowHandle"] = hWnd.ToString();
            window = root.CreateRenderWindow("Simple Mogre Form Window", 0, 0, false, misc);
            ResourceGroupManager.Singleton.InitialiseAllResourceGroups();

            //----------------------------------------------------- 
            // 4 Create the SceneManager
            // 
            //		ST_GENERIC = octree
            //		ST_EXTERIOR_CLOSE = simple terrain
            //		ST_EXTERIOR_FAR = nature terrain (depreciated)
            //		ST_EXTERIOR_REAL_FAR = paging landscape
            //		ST_INTERIOR = Quake3 BSP
            //----------------------------------------------------- 
            sceneMgr = root.CreateSceneManager(SceneType.ST_GENERIC, "SceneMgr");
            sceneMgr.AmbientLight = new ColourValue(0.5f, 0.5f, 0.5f);

            //----------------------------------------------------- 
            // 5 Create the camera 
            //----------------------------------------------------- 
            camera = sceneMgr.CreateCamera("SimpleCamera"); 
	        camera.Position = new Vector3(0f,0f,100f);
	        // Look back along -Z
	        camera.LookAt(new Vector3(0f,0f,-300f));
	        camera.NearClipDistance = 5;

            viewport = window.AddViewport(camera);
            viewport.BackgroundColour = new ColourValue(0.0f, 0.0f, 0.0f, 1.0f);


            Entity ent = sceneMgr.CreateEntity("ogre", "ogrehead.mesh");
            SceneNode node = sceneMgr.RootSceneNode.CreateChildSceneNode("ogreNode");
            node.AttachObject(ent);
        }

        public void Paint()
        {
            root.RenderOneFrame();
        }

        public void Dispose()
        {
            if (root != null)
            {
                root.Dispose();
                root = null;
            }
        }
    }

}


