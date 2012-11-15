using System;
using System.Collections.Generic;
using System.Text;
using ColorEx = Mogre.ColourValue;


namespace Mogre.Demo.MogreForm {
    public class LightGrassWibbler  {
        protected Light light;
        protected Billboard billboard;
        protected ColorEx colorRange = new ColorEx();
        protected ColorEx halfColor = new ColorEx();
        protected float minSize;
        protected float sizeRange;
        protected float intensity;

        public LightGrassWibbler(Light light, Billboard billboard, ColorEx minColor, ColorEx maxColor, int minSize, int maxSize) {
            this.light = light;
            this.billboard = billboard;

            this.colorRange.r = (maxColor.r - minColor.r) * 0.5f;
            this.colorRange.g = (maxColor.g - minColor.g) * 0.5f;
            this.colorRange.b = (maxColor.b - minColor.b) * 0.5f;

            this.halfColor.r = (minColor.r + colorRange.r); // 2;
            this.halfColor.g = (minColor.g + colorRange.g); // 2;
            this.halfColor.b = (minColor.b + colorRange.b); // 2;

            this.minSize = minSize;
            this.sizeRange = maxSize - minSize;
        }

        #region IControllerValue<float> Members

        public float Value {
            get {
                return intensity;
            }
            set {
                intensity = value;

                ColorEx newColor = new ColorEx();
                //atenuate the brightness of the light
                newColor.r = halfColor.r + (colorRange.r * intensity);
                newColor.g = halfColor.g + (colorRange.g * intensity);
                newColor.b = halfColor.b + (colorRange.b * intensity);

                this.light.DiffuseColour = newColor;
                this.billboard.Colour = newColor;

                float newSize = minSize + (intensity * sizeRange);
                this.billboard.SetDimensions(newSize, newSize);
            }
        }

        #endregion IControllerValue<float> Members
    }
    public class Grass {
        protected const float GRASS_HEIGHT = 300;
        protected const float GRASS_WIDTH = 250;
        protected const string GRASS_MESH_NAME = "grassblades";
        protected string GRASS_MATERIAL = "Examples/GrassBlades";
        protected const int OFFSET_PARAM = 999;
        protected float extraOffset = 0.1f;
        protected float randomRange = 60;
        protected bool backward = false;

        protected Light Light;
        protected SceneNode LightNode;
        protected AnimationState AnimState;

        protected readonly ColorEx MinLightColour = new ColorEx(0.5f, 0.1f, 0.0f);

        protected readonly ColorEx MaxLightColour = new ColorEx(1.0f, 0.6f, 0.0f);

        protected int MinFlareSize = 40;
        protected int MaxFlareSize = 80;
        protected StaticGeometry StaticGeom;
        //protected SceneNode HeadNode;
        //增加
        SceneManager scene;
        public Grass(SceneManager sceneMgr) {
            scene = sceneMgr;
        }
        public void CreateScene() {
            scene.SetSkyBox(true, "Examples/SpaceSkyBox");

            SetupLighting();

            Plane plane = new Plane();
            plane.normal = Vector3.UNIT_Y;
            plane.d = 0;

            MeshManager.Singleton.CreatePlane("MyPlane", ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME, plane, 14500, 14500, 10, 10, true, 1, 50, 50, Vector3.UNIT_Z);

            Entity planeEnt = scene.CreateEntity("plane", "MyPlane");
            planeEnt.SetMaterialName("Examples/GrassFloor");
            planeEnt.CastShadows = false;

            scene.RootSceneNode.CreateChildSceneNode().AttachObject(planeEnt);

            Vector3 minV = new Vector3(-2000, 0, -2000);
            Vector3 maxV = new Vector3(2000, 0, 2000);

            CreateGrassMesh();

            Entity e = scene.CreateEntity("1", GRASS_MESH_NAME);
            StaticGeometry s = scene.CreateStaticGeometry("bing");//, 1);
            s.RegionDimensions = new Vector3(1000, 1000, 1000);
            s.Origin = new Vector3(-500, 500, -500); //Set the region origin so the centre is at 0 world

            for (int x = -1950; x < 1950; x += 150) {
                for (int z = -1950; z < 1950; z += 150) {
                    Vector3 pos = new Vector3(x + Math.RangeRandom(-25, 25), 0, z + Math.RangeRandom(-25, 25));

                    Quaternion orientation =new Quaternion();
                    orientation.FromAngleAxis(Math.RangeRandom(0, 359), Vector3.UNIT_Y);

                    Vector3 scale = new Vector3(1, Math.RangeRandom(0.85f, 1.15f), 1);

                    s.AddEntity(e, pos, orientation, scale);
                }
            }
            s.Build();
            StaticGeom = s;

            //Mesh mesh = MeshManager.Singleton.Load("ogrehead.mesh", ResourceGroupManager.DefaultResourceGroupName);

            //short src, dest;
            //if (!mesh.SuggestTangentVectorBuildParams( VertexElementSemantic.VES_POSITION, out src, out dest)) {
            //    mesh.BuildTangentVectors( VertexElementSemantic.VES_POSITION,src, dest);
            //}

            //e = scene.CreateEntity("head", "ogrehead.mesh");
            //e.SetMaterialName("Examples/OffsetMapping/Specular");

            //HeadNode = scene.RootSceneNode.CreateChildSceneNode();
            //HeadNode.AttachObject(e);
            //HeadNode.Scale = new Vector3(7, 7, 7);
            //HeadNode.Position = new Vector3(0, 200, 0);

            //if (e.GetSubEntity(0).NormalizeNormals == false) {
            //    LogManager.Singleton.LogMessage("aie aie aie");
            //}
            Root.Singleton.RenderSystem.SetNormaliseNormals(true);

            //camera.Move(new Vector3(0, 350, 0));
        }

        private void CreateGrassMesh() {
            // Each grass section is 3 planes at 60 degrees to each other
            // Normals point straight up to simulate correct lighting
            Mesh msh = MeshManager.Singleton.CreateManual(GRASS_MESH_NAME, ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME, null);

            SubMesh sm = msh.CreateSubMesh();
            sm.useSharedVertices = false;
            sm.vertexData = new VertexData();
            sm.vertexData.vertexStart = 0;
            sm.vertexData.vertexCount = 12;

            VertexDeclaration dcl = sm.vertexData.vertexDeclaration;
            uint offset = 0;

            dcl.AddElement(0, offset, VertexElementType.VET_FLOAT3, VertexElementSemantic.VES_POSITION);
            offset += VertexElement.GetTypeSize(VertexElementType.VET_FLOAT3);
            dcl.AddElement(0, offset, VertexElementType.VET_FLOAT3, VertexElementSemantic.VES_NORMAL);
            offset += VertexElement.GetTypeSize(VertexElementType.VET_FLOAT3);
            dcl.AddElement(0, offset, VertexElementType.VET_FLOAT2, VertexElementSemantic.VES_TEXTURE_COORDINATES);
            offset += VertexElement.GetTypeSize(VertexElementType.VET_FLOAT2);

            HardwareVertexBufferSharedPtr vbuf = HardwareBufferManager.Singleton.CreateVertexBuffer(offset, 12, HardwareBuffer.Usage.HBU_STATIC_WRITE_ONLY);

            int i;
            unsafe {
                float* pData = (float*)(vbuf.Lock(HardwareBuffer.LockOptions.HBL_DISCARD));

                Vector3 baseVec = new Vector3(GRASS_WIDTH / 2, 0, 0);
                Vector3 vec = baseVec;
                Quaternion rot = new Quaternion();
                rot.FromAngleAxis(Math.DegreesToRadians(60), Vector3.UNIT_Y);

                for (i = 0; i < 3; ++i) {
                    //position
                    *pData++ = -vec.x;
                    *pData++ = GRASS_HEIGHT;
                    *pData++ = -vec.z;
                    // normal
                    *pData++ = 0;
                    *pData++ = 1;
                    *pData++ = 0;
                    // uv
                    *pData++ = 0;
                    *pData++ = 0;

                    // position
                    *pData++ = vec.x;
                    *pData++ = GRASS_HEIGHT;
                    *pData++ = vec.z;
                    // normal
                    *pData++ = 0;
                    *pData++ = 1;
                    *pData++ = 0;
                    // uv
                    *pData++ = 1;
                    *pData++ = 0;

                    // position
                    *pData++ = -vec.x;
                    *pData++ = 0;
                    *pData++ = -vec.z;
                    // normal
                    *pData++ = 0;
                    *pData++ = 1;
                    *pData++ = 0;
                    // uv
                    *pData++ = 0;
                    *pData++ = 1;

                    // position
                    *pData++ = vec.x;
                    *pData++ = 0;
                    *pData++ = vec.z;
                    // normal
                    *pData++ = 0;
                    *pData++ = 1;
                    *pData++ = 0;
                    // uv
                    *pData++ = 1;
                    *pData++ = 1;

                    vec = rot * vec;
                } //for
            } //unsafe

            vbuf.Unlock();

            sm.vertexData.vertexBufferBinding.SetBinding(0, vbuf);
            sm.indexData.indexCount = 6 * 3;
            sm.indexData.indexBuffer = HardwareBufferManager.Singleton.CreateIndexBuffer(HardwareIndexBuffer.IndexType.IT_16BIT, 6 * 3, HardwareBuffer.Usage.HBU_STATIC_WRITE_ONLY);

            unsafe {
                ushort* pI = (ushort*)(sm.indexData.indexBuffer.Lock(HardwareBuffer.LockOptions.HBL_DISCARD));

                for (i = 0; i < 3; ++i) {
                    int off = i * 4;
                    *pI++ = (ushort)(off);
                    *pI++ = (ushort)(off + 3);
                    *pI++ = (ushort)(off + 1);

                    *pI++ = (ushort)(off + 0);
                    *pI++ = (ushort)(off + 2);
                    *pI++ = (ushort)(off + 3);
                }
            }

            sm.indexData.indexBuffer.Unlock();
            sm.SetMaterialName(GRASS_MATERIAL);

            msh.Load();
        }

        private void SetupLighting() {
            scene.AmbientLight = new ColorEx(0.2f, 0.2f, 0.2f);
            Light = scene.CreateLight("Light2");
            Light.DiffuseColour = MinLightColour;
            Light.SetAttenuation(8000, 1, 0.0005f, 0);
            Light.SpecularColour = new ColorEx(1, 1, 1);

            LightNode = scene.RootSceneNode.CreateChildSceneNode("MovingLightNode");
            LightNode.AttachObject(Light);
            //create billboard set

            BillboardSet bbs = scene.CreateBillboardSet("lightbbs", 1);
            bbs.SetMaterialName("Examples/Flare");
            Billboard bb = bbs.CreateBillboard(new Vector3(0, 0, 0), MinLightColour);
            LightNode.AttachObject(bbs);

            mLightController = new LightGrassWibbler(Light, bb, MinLightColour, this.MaxLightColour, MinFlareSize, MaxFlareSize);

            // create controller, after this is will get updated on its own
            //WaveformControllerFunction func = new WaveformControllerFunction(WaveformType.Sine, 0.0f, 0.5f);

            //ControllerManager.Instance.CreateController(val, func);

            LightNode.Position = new Vector3(300, 250, -300);

            Animation anim = scene.CreateAnimation("LightTrack", 20);
            //Spline it for nce curves
            anim.SetInterpolationMode(Animation.InterpolationMode.IM_SPLINE);// = Mogre.Animation.InterpolationMode.IM_SPLINE;
            //create a srtack to animte the camera's node
            NodeAnimationTrack track = anim.CreateNodeTrack(0, LightNode);
            //setup keyframes
            TransformKeyFrame key = track.CreateNodeKeyFrame(0);
            key.Translate = new Vector3(300, 550, -300);
            key = track.CreateNodeKeyFrame(2); //B
            key.Translate = new Vector3(150, 600, -250);
            key = track.CreateNodeKeyFrame(4); //C
            key.Translate = new Vector3(-150, 650, -100);
            key = track.CreateNodeKeyFrame(6); //D
            key.Translate = new Vector3(-400, 500, -200);
            key = track.CreateNodeKeyFrame(8); //E
            key.Translate = new Vector3(-200, 500, -400);
            key = track.CreateNodeKeyFrame(10); //F
            key.Translate = new Vector3(-100, 450, -200);
            key = track.CreateNodeKeyFrame(12); //G
            key.Translate = new Vector3(-100, 400, 180);
            key = track.CreateNodeKeyFrame(14); //H
            key.Translate = new Vector3(0, 250, 600);
            key = track.CreateNodeKeyFrame(16); //I
            key.Translate = new Vector3(100, 650, 100);
            key = track.CreateNodeKeyFrame(18); //J
            key.Translate = new Vector3(250, 600, 0);
            key = track.CreateNodeKeyFrame(20); //K == A
            key.Translate = new Vector3(300, 550, -300);
            // Create a new animation state to track this

            AnimState = scene.CreateAnimationState("LightTrack");
            AnimState.Enabled = true;
        }

        internal void OnFrameStarted(FrameEvent evt) {

            // animate Light Wibbler
            AnimState.AddTime(evt.timeSinceLastFrame);

            randomRange = Math.RangeRandom(20, 100);

            if (!backward) {
                extraOffset += 0.5f;
                if (extraOffset > randomRange) {
                    backward = true;
                }
            }
            if (backward) {
                extraOffset -= 0.5f;
                if (extraOffset < 0.02f) {
                    backward = false;
                }
            }

            // we are animating the static mesh ( Entity ) here with a simple offset
            waveGrass(evt.timeSinceLastFrame);
        } //end function

        //
        LightGrassWibbler mLightController;
        public void cleanupContent() {
            //ControllerManager.Instance.DestroyController(mLightController);
            MeshManager.Singleton.Remove("plane");
            MeshManager.Singleton.Remove(GRASS_MESH_NAME);
        }
        void waveGrass(float timeElapsed)
	{
		 float xinc = Math.PI * 0.3f;
		 float zinc = Math.PI * 0.44f;
		 float xpos = Math.RangeRandom(-Math.PI, Math.PI);
		 float zpos = Math.RangeRandom(-Math.PI, Math.PI);
		 Vector4 offset=new Vector4(0, 0, 0, 0);

		xpos += xinc * timeElapsed;
		zpos += zinc * timeElapsed;

		// update vertex program parameters by binding a value to each renderable
		StaticGeometry.RegionIterator regs =  StaticGeom.GetRegionIterator();
		while (regs.MoveNext())
		{
			StaticGeometry.Region reg = regs.Current;

			// a little randomness
			xpos += reg.Centre.x * 0.001f;
			zpos += reg.Centre.z * 0.001f;
			offset.x = Math.Sin(xpos) * 4;
			offset.z = Math.Sin(zpos) * 4;

			StaticGeometry.Region.LODIterator lods = reg.GetLODIterator();
			while (lods.MoveNext())
			{
				StaticGeometry.LODBucket.MaterialIterator mats = lods.Current.GetMaterialIterator();
				while (mats.MoveNext())
				{					
                    IEnumerator<StaticGeometry.MaterialBucket> geoms=mats.GetEnumerator();
                    while (geoms.MoveNext()) {
                        geoms.Current.CurrentTechnique.GetPass(0).GetGeometryProgramParameters().SetConstant(999, offset);
                    }
					//while (geoms.hasMoreElements()) geoms.getNext()->setCustomParameter(999, offset);
				}
                
			}
		}
	}

    }
}
