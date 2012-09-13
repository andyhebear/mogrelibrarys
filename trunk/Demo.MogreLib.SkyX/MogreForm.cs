using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mogre;
using MogreLib.SkyX;

namespace Mogre.Demo.MogreForm {
    public partial class MogreForm : Form {
        protected OgreWindow mogreWin;

        public MogreForm() {
            InitializeComponent();
            this.Disposed += new EventHandler(MogreForm_Disposed);

            mogreWin = new OgreWindow(new Point(100, 30), mogrePanel.Handle);
            mogreWin.InitMogre();
            CreateSkyX();
            mogreWin.root.FrameStarted += new FrameListener.FrameStartedHandler(root_FrameStarted);
        }
        private SkyX _skyX;
        private bool _showInformation = false;
        private float _keyDelay = 1.0f;

        private void CreateSkyX() {
            // Create SkyX
            _skyX = new SkyX(mogreWin.sceneMgr, mogreWin.camera);
            _skyX.Create();
            _skyX.VCloudsManager.Create();

            ////// A little change to default atmosphere settings :)
            AtmosphereManager.AtmosphereOptions opt = (AtmosphereManager.AtmosphereOptions)_skyX.AtmosphereManager.Options.Clone();
            opt.RayleighMultiplier = 0.0045f;
            _skyX.AtmosphereManager.Options = opt;
            // Add our ground atmospheric scattering pass to terrain material
            //var terrainMaterial = (Material)MaterialManager.Instance.GetByName("Terrain");
            //if (terrainMaterial != null)
            //{
            //    var pass = terrainMaterial.GetTechnique(0).CreatePass();
            //    _skyX.GpuManager.AddGroundPass(pass, 5000, SceneBlendType.TransparentColor);
            //}
            ////// Create our terrain
            //scene.SetWorldGeometry("Terrain.xml");

            //// Add a basic cloud layer
            _skyX.CloudsManager.Add(new CloudLayer.Options(/* Default options */));
        }
        bool root_FrameStarted(FrameEvent e) {
            //_skyX.AtmosphereManager.Update(skyXOptions, true);
            _skyX.Update(e.timeSinceLastFrame);
            //_skyX.VCloudsManager.Update(e.TimeSinceLastFrame);
            _keyDelay -= e.timeSinceLastFrame;

            // Update terrain material
            //var terrainMaterial = (MaterialPtr)MaterialManager.Singleton.GetByName("Terrain");
            //if (terrainMaterial != null)
            //    terrainMaterial.GetTechnique(0).GetPass(0).GetFragmentProgramParameters().SetNamedConstant("uLightY", -_skyX.AtmosphereManager.SunDirection.y);
            //debugText = this.camera.DerivedPosition.ToString();
            return true;
        }
        //protected override void OnLoad(EventArgs e) {
        //    this.Show();
        //    base.OnLoad(e);
        //    while (true) {
        //        Application.DoEvents();
        //        this.Invalidate();
        //    }
        //}
        protected override void OnKeyUp(KeyEventArgs e) {
            // Time
            float TimeSinceLastFrame = 0.2f;
            _skyX.TimeMultiplier = !_showInformation ? 0.1f : 0.0f;
            if (_skyX.TimeMultiplier != 0) {

            }
            // var 
            var skyXOptions = (AtmosphereManager.AtmosphereOptions)_skyX.AtmosphereManager.Options.Clone();

            // Show/Hide information
            if (e.KeyCode == Keys.F11 && _keyDelay < 0) {
                _showInformation = !_showInformation;

                _keyDelay = 0.25f;
            }
            if (e.KeyCode == Keys.D1 && !(e.Control || e.Shift)) {
                _skyX.TimeMultiplier = 1.0f;
            }
            if (e.KeyCode == Keys.D1 && (e.Control || e.Shift)) {
                _skyX.TimeMultiplier = -1.0f;
            }
            // Rayleigh multiplier
            if (e.KeyCode == Keys.D2 && !(e.Control || e.Shift)) {

                skyXOptions.RayleighMultiplier += TimeSinceLastFrame * 0.025f;
            }
            if (e.KeyCode == Keys.D2 && (e.Control || e.Shift)) {
                skyXOptions.RayleighMultiplier -= TimeSinceLastFrame * 0.025f;
            }

            // Mie multiplier
            if (e.KeyCode == Keys.D3 && !(e.Control || e.Shift)) {
                skyXOptions.MieMultiplier += TimeSinceLastFrame * 0.025f;
            }
            if (e.KeyCode == Keys.D3 && (e.Control || e.Shift)) {
                skyXOptions.MieMultiplier -= TimeSinceLastFrame * 0.025f;
            }
            // Exposure
            if (e.KeyCode == Keys.D4 && !(e.Control || e.Shift)) {
                skyXOptions.Exposure += TimeSinceLastFrame * 0.5f;
            }
            if (e.KeyCode == Keys.D4 && (e.Control || e.Shift)) {
                skyXOptions.Exposure -= TimeSinceLastFrame * 0.5f;
            }
            // Inner radius
            if (e.KeyCode == Keys.D5 && !(e.Control || e.Shift)) {
                skyXOptions.InnerRadius += TimeSinceLastFrame * 0.25f;
            }
            if (e.KeyCode == Keys.D5 && (e.Control || e.Shift)) {
                skyXOptions.InnerRadius -= TimeSinceLastFrame * 0.25f;
            }
            // Outer radius
            if (e.KeyCode == Keys.D6 && !(e.Control || e.Shift)) {
                skyXOptions.OuterRadius += TimeSinceLastFrame * 0.25f;
            }
            if (e.KeyCode == Keys.D6 && (e.Control || e.Shift)) {
                skyXOptions.OuterRadius -= TimeSinceLastFrame * 0.25f;
            }
            // Number of samples
            if (_keyDelay < 0 && e.KeyCode == Keys.D7 && !(e.Control || e.Shift)) {
                skyXOptions.NumberOfSamples++;
                _keyDelay = 0.25f;
            }
            if (_keyDelay < 0 && e.KeyCode == Keys.D7 && (e.Control || e.Shift)) {
                skyXOptions.NumberOfSamples--;
                _keyDelay = 0.25f;
            }
            // Outer radius
            if (_keyDelay < 0 && e.KeyCode == Keys.D8 && !(e.Control || e.Shift)) {
                skyXOptions.HeightPosition += TimeSinceLastFrame * 0.05f;
            }
            if (_keyDelay < 0 && e.KeyCode == Keys.D8 && (e.Control || e.Shift)) {
                skyXOptions.HeightPosition -= TimeSinceLastFrame * 0.05f;
            }
            if (e.KeyCode == Keys.Space && _keyDelay < 0) {
                mogreWin.camera.Position = new Vector3(0, 900, 0);
                _keyDelay = 0.25f;
            }
            // 

            // _skyX.VCloudsManager.WindSpeed = _skyX.VCloudsManager.WindSpeed + 100.0f * e.TimeSinceLastFrame;

            //_textArea.Text = getConfigStringFromSkyXAtmosphereOptions(skyXOptions);
            _skyX.AtmosphereManager.Options = skyXOptions;
            base.OnKeyUp(e);
        }
        private string getConfigStringFromSkyXAtmosphereOptions(AtmosphereManager.AtmosphereOptions Options) {
            var hour = Options.Time.x;
            var min = (int)((Options.Time.x - hour) * 60);

            var timeStr = hour + ":" + min;
            var str = "MogreLib SkyX Plugin demo (Press F1 to show/hide information)" + (_showInformation ? " - Simuation paused - \n" : "\n-------------------------------------------------------------\nTime: " + timeStr + "\n");

            if (_showInformation) {
                str += "-------------------------------------------------------------\n";
                str += "Time: " + timeStr + " [1, Shift+1] (+/-).\n";
                str += "Rayleigh multiplier: " + Options.RayleighMultiplier + " [2, Shift+2] (+/-).\n";
                str += "Mie multiplier: " + Options.MieMultiplier + " [3, Shift+3] (+/-).\n";
                str += "Exposure: " + Options.Exposure + " [4, Shift+4] (+/-).\n";
                str += "Inner radius: " + Options.InnerRadius + " [5, Shift+5] (+/-).\n";
                str += "Outer radius: " + Options.OuterRadius + " [6, Shift+6] (+/-).\n";
                str += "Number of samples: " + Options.NumberOfSamples + " [7, Shift+7] (+/-).\n";
                str += "Height position: " + Options.HeightPosition + " [8, Shift+8] (+/-).\n";

                if (_skyX.VCloudsManager.IsCreated) {
                    str += "Wind direction: " + _skyX.VCloudsManager.VClouds.WindDirection.ValueDegrees.ToString() + " degrees [0, Shift+0] (+/-).\n";
                    str += "Clouds roughness: " + _skyX.VCloudsManager.VClouds.NoiseScale + " [p, Shift+p] (+/-).\n";
                }
            }

            return str;
        }
        OverlayElement _textArea;
        // Create text area for SkyX parameters
        private void _createTextArea() {
            // Create a panel
            OverlayContainer panel = (OverlayContainer)OverlayManager.Singleton.CreateOverlayElement("Panel", "SkyXParametersPanel");
            panel.MetricsMode = GuiMetricsMode.GMM_PIXELS;
            panel.SetPosition(10, 10);
            panel.SetDimensions(400, 400);

            // Create a text area
            _textArea = OverlayManager.Singleton.CreateOverlayElement("TextArea", "SkyXParametersTextArea");
            _textArea.MetricsMode = GuiMetricsMode.GMM_PIXELS;
            _textArea.SetPosition(0, 0);
            _textArea.SetDimensions(100, 100);
            // _textArea.Text = "MogreLib SkyX plugin demo";
            _textArea._setHeight(16);
            //_textArea.FontName = "BlueHighway";
            // _textArea.ColorBottom = new ColorEx(0.3f, 0.5f, 0.3f);
            //_textArea.ColorTop = new ColorEx(0.5f, 0.7f, 0.5f);

            // Create an overlay, and add the panel
            Overlay overlay = OverlayManager.Singleton.Create("OverlayName");
            overlay.Add2D(panel);

            // Add the text area to the panel
            panel.AddChild(_textArea);

            // Show the overlay
            overlay.Show();
        }
        private void MogreForm_Paint(object sender, PaintEventArgs e) {
            mogreWin.Paint();
        }

        void MogreForm_Disposed(object sender, EventArgs e) {
            //_skyX.Remove();
            mogreWin.Dispose();
        }
    }

    public class OgreWindow {
        public Root root;
        public SceneManager sceneMgr;

        internal Camera camera;
        internal Viewport viewport;
        internal RenderWindow window;
        protected Point position;
        protected IntPtr hWnd;

        public OgreWindow(Point origin, IntPtr hWnd) {
            position = origin;
            this.hWnd = hWnd;
        }

        public void InitMogre() {

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
            while (seci.MoveNext()) {
                secName = seci.CurrentKey;
                ConfigFile.SettingsMultiMap settings = seci.Current;
                foreach (KeyValuePair<string, string> pair in settings) {
                    typeName = pair.Key;
                    archName = pair.Value;
                    ResourceGroupManager.Singleton.AddResourceLocation(archName, typeName, secName);
                }
            }

            //----------------------------------------------------- 
            // 3 Configures the application and creates the window
            //----------------------------------------------------- 
            bool foundit = false;
            foreach (RenderSystem rs in root.GetAvailableRenderers()) {
                root.RenderSystem = rs;
                String rname = root.RenderSystem.Name;
                if (rname == "Direct3D9 Rendering Subsystem") {
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
            camera.Position = new Vector3(0f, 0f, 100f);
            // Look back along -Z
            camera.LookAt(new Vector3(0f, 0f, -300f));
            camera.NearClipDistance = 5;

            viewport = window.AddViewport(camera);
            viewport.BackgroundColour = new ColourValue(0.0f, 0.0f, 0.0f, 1.0f);


            Entity ent = sceneMgr.CreateEntity("ogre", "ogrehead.mesh");
            SceneNode node = sceneMgr.RootSceneNode.CreateChildSceneNode("ogreNode");
            node.AttachObject(ent);
        }

        public void Paint() {
            root.RenderOneFrame();
        }

        public void Dispose() {
            if (root != null) {
                //root.Shutdown();
                //root.Dispose();
                //root = null;
            }
        }
    }

}


