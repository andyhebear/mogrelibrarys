#region MIT/X11 License
// This file is part of the MogreLib.SkyX project
//
// Copyright (c) 2009 Michael Cummings <cummings.michael@gmail.com>
// Copyright (c) 2009 Bostich <bostich83@googlemail.com>

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// MogreLib.SkyX is a reimplementation of the SkyX project for .Net/Mono
// SkyX is Copyright (C) 2009 Xavier Verguín González <xavierverguin@hotmail.com> <xavyiy@gmail.com>

// MogreLib.SkyX is Copyright (C) 2012 rains <andyhebear@hotmail.com> <andyhebear@gmail.com>
// MogreLib.SkyX svn: http://mogrelibrarys.googlecode.com/svn/trunk/  Author Blog http://hi.baidu.com/andyhebear/    http://hi.baidu.com/rainssoft/
#endregion MIT/X11 License

using System;
//using MogreLib.Core;
//using MogreLib.Graphics;
//using Utility = Mogre.Math;
//using MogreLib.Math;
using Mogre;
using MogreLib.Math;

namespace MogreLib.SkyX
{
    public class MeshManager : IDisposable
    {
        #region - PosUVVertex -
        /// <summary>
        /// Vertex struct for position and tex coords data.
        /// </summary>
        public struct PosUVVertex
        {
            /// <summary>
            /// X position of the vertex.
            /// </summary>
            public float X;
            /// <summary>
            /// Y position of the vertex
            /// </summary>
            public float Y;
            /// <summary>
            /// Z postion of the vertex
            /// </summary>
            public float Z;
            /// <summary>
            /// x normal of the vertex
            /// </summary>
            public float NX;
            /// <summary>
            /// x normal of the vertex
            /// </summary>
            public float NY;
            /// <summary>
            /// z normal of the vertex
            /// </summary>
            public float NZ;
            /// <summary>
            /// Texture coordinate U
            /// </summary>
            public float U;
            /// <summary>
            /// Texture coordinate V
            /// </summary>
            public float V;
            /// <summary>
            /// Opacity
            /// </summary>
            public float Opacity;

            public static int SizeInBytes = (4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4) * 9;
        }
        #endregion

        /// <summary>
        /// was create() allready called?
        /// </summary>
        private bool _created;
        /// <summary>
        /// Mesh
        /// </summary>
        private Mesh _mesh;
        /// <summary>
        /// submesh
        /// </summary>
        private SubMesh _subMesh;
        /// <summary>
        /// entity
        /// </summary>
        private Entity _entity;
        /// <summary>
        /// Vertex buffer.
        /// </summary>
        //private HardwareVertexBuffer _vertexBuffer;
        private HardwareVertexBufferSharedPtr _vertexBuffer;
        /// <summary>
        /// Index buffer.
        /// </summary>
       // private HardwareIndexBuffer _indexBuffer;
        private HardwareIndexBufferSharedPtr  _indexBuffer;
        /// <summary>
        /// Vertices.
        /// </summary>
        private PosUVVertex[] _vertices;
        /// <summary>
        /// 
        /// </summary>
        private int _circles;
        /// <summary>
        /// 
        /// </summary>
        private int _steps;
        /// <summary>
        /// 
        /// </summary>
        private bool _smoothSkyDomeFading;
        /// <summary>
        /// 
        /// </summary>
        private float _skydomeFadingPercent;
        /// <summary>
        /// 
        /// </summary>
        private SceneNode _sceneNode;
        /// <summary>
        /// 
        /// </summary>
        private string _materialName;
        /// <summary>
        /// 
        /// </summary>
        private SkyX _skyX;

        /// <summary>
        /// Get's a reference of the main SkyX
        /// </summary>
        public SkyX SkyX
        {
            get
            {
                return _skyX;
            }
            private set
            {
                _skyX = value;
            }
        }
        /// <summary>
        /// Get's a reference of the used mesh
        /// </summary>
        public Mesh Mesh
        {
            get
            {
                return _mesh;
            }
            private set
            {
                _mesh = value;
            }
        }
        /// <summary>
        /// Get's a reference to the submesh
        /// </summary>
        public SubMesh SubMesh
        {
            get
            {
                return _subMesh;
            }
            private set
            {
                _subMesh = value;
            }
        }
        /// <summary>
        /// Get's a reference to the entity
        /// </summary>
        public Entity Entity
        {
            get
            {
                return _entity;
            }
            private set
            {
                _entity = value;
            }
        }
        /// <summary>
        /// Get's a reference to the SmoothSkydomeFading property
        /// </summary>
        public bool SmoothSkydomeFading
        {
            get
            {
                return _smoothSkyDomeFading;
            }
            private set
            {
                _smoothSkyDomeFading = value;
            }
        }

        /// <summary>
        /// Get's a reference to the SkydomeFadingPercent property
        /// </summary>
        public float SkydomeFadingPercent
        {
            get
            {
                return _skydomeFadingPercent;
            }
            private set
            {
                _skydomeFadingPercent = value;
            }
        }
        /// <summary>
        /// Get's or set's the MaterialPtr name.
        /// </summary>
        public string MaterialName
        {
            get
            {
                return _materialName;
            }
            set
            {
                SetMaterialName(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public HardwareVertexBufferSharedPtr HardwareVertexBuffer
        {
            get
            {
                return _vertexBuffer;
            }
            private set
            {
                _vertexBuffer = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public HardwareIndexBufferSharedPtr HardwareIndexBuffer
        {
            get
            {
                return _indexBuffer;
            }
            private set
            {
                _indexBuffer = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public SceneNode SceneNode
        {
            get
            {
                return _sceneNode;
            }
            private set
            {
                _sceneNode = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsCreated
        {
            get
            {
                return _created;
            }
            private set
            {
                _created = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Steps
        {
            get
            {
                return _steps;
            }
            private set
            {
                _steps = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Circles
        {
            get
            {
                return _circles;
            }
            private set
            {
                _circles = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public float SkydomeRadius
        {
            get
            {
                return this.SkyX.Camera.FarClipDistance * 0.95f;
            }
        }
        #region Construction and Destruction

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skyX"></param>
        public MeshManager(SkyX skyX)
        {
            this.SkyX = skyX;
            this.SmoothSkydomeFading = true;
            this.SkydomeFadingPercent = 0.05f;
            this.MaterialName = "_NULL_";
            this.Steps = 70;
            this.Circles = 80;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Remove();
        }
        #endregion Construction and Destruction

        /// <summary>
        /// 
        /// </summary>
        public void Create()
        {
            if (this.IsCreated)
            {
                return;
            }

            // Create mesh and submesh
            this.Mesh = Mogre.MeshManager.Singleton.CreateManual("SkyXMesh", SkyX.SkyXResourceGroup,null);
            this.SubMesh = this.Mesh.CreateSubMesh();
            this.SubMesh.useSharedVertices = false;

            // Create mesh geometry
            CreateGeometry();

            // End mesh creation
            this.Mesh.Load();
            this.Mesh.Touch();

            this.Entity = this.SkyX.SceneManager.CreateEntity("SkyXMeshEnt", "SkyXMesh");
            SetMaterialName(this.MaterialName);
            this.Entity.CastShadows = false;
            this.Entity.RenderQueueGroup = (byte)(RenderQueueGroupID.RENDER_QUEUE_SKIES_EARLY + 1);

            this.SceneNode = this.SkyX.SceneManager.RootSceneNode.CreateChildSceneNode();
            this.SceneNode.ShowBoundingBox = false;
            this.SceneNode.AttachObject(this.Entity);
            this.SceneNode.Position = this.SkyX.Camera.DerivedPosition;

            this.IsCreated = true;

            UpdateGeometry();
        }
        //MeshPtr CreateMesh(string Name, string Group, IndexData IndexDataArg, VertexData VertexDataArg, AxisAlignedBox BoundingBox) {
        //    Mogre.Mesh  Mesh = Mogre.MeshManager.Singleton.CreateManual(Name, Group,null);
        //    SubMesh SubMesh = Mesh.CreateSubMesh();

        //    //Shallow copy the IndexBuffer argument into the SubMesh's indexData property
        //    SubMesh.indexData.indexBuffer = IndexDataArg.indexBuffer;
        //    SubMesh.indexData.indexCount = IndexDataArg.indexCount;

        //    //Deep copy the VertexData argument into the Mesh's sharedVertexData
        //    SubMesh.useSharedVertices = true;
        //    Mesh.sharedVertexData = new VertexData();
        //    Mesh.sharedVertexData.vertexBufferBinding.SetBinding(0, VertexDataArg.vertexBufferBinding.GetBuffer(0));
        //    Mesh.sharedVertexData.vertexDeclaration = VertexDataArg.vertexDeclaration.Clone();
        //    Mesh.sharedVertexData.vertexCount = VertexDataArg.vertexCount;

        //    Mesh._setBounds(BoundingBox);

        //    Mesh.Load();

        //    return Mesh;
        //}
        unsafe static public void CreateSphere(string strName, float r, int nRings, int nSegments) {

            MeshPtr pSphere = Mogre.MeshManager.Singleton.CreateManual(strName, ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME);
            SubMesh pSphereVertex = pSphere.CreateSubMesh();

            pSphere.sharedVertexData = new VertexData();
            VertexData vertexData = pSphere.sharedVertexData;

            // define the vertex format
            VertexDeclaration vertexDecl = vertexData.vertexDeclaration;
            uint currOffset = 0;
            // positions
            vertexDecl.AddElement(0, currOffset, VertexElementType.VET_FLOAT3, VertexElementSemantic.VES_POSITION);
            currOffset += VertexElement.GetTypeSize(VertexElementType.VET_FLOAT3);
            // normals
            vertexDecl.AddElement(0, currOffset, VertexElementType.VET_FLOAT3, VertexElementSemantic.VES_NORMAL);
            currOffset += VertexElement.GetTypeSize(VertexElementType.VET_FLOAT3);
            // two dimensional texture coordinates
            vertexDecl.AddElement(0, currOffset, VertexElementType.VET_FLOAT2, VertexElementSemantic.VES_TEXTURE_COORDINATES, 0);
            currOffset += VertexElement.GetTypeSize(VertexElementType.VET_FLOAT2);

            // allocate the vertex buffer
            vertexData.vertexCount = (uint)((nRings + 1) * (nSegments + 1));
            HardwareVertexBufferSharedPtr vBuf = HardwareBufferManager.Singleton.CreateVertexBuffer(vertexDecl.GetVertexSize(0), vertexData.vertexCount, HardwareBuffer.Usage.HBU_STATIC_WRITE_ONLY, false);
            VertexBufferBinding binding = vertexData.vertexBufferBinding;
            binding.SetBinding(0, vBuf);
            float* pVertex = (float*)vBuf.Lock(HardwareBuffer.LockOptions.HBL_DISCARD);

            // allocate index buffer
            pSphereVertex.indexData.indexCount = (uint)(6 * nRings * (nSegments + 1));
            pSphereVertex.indexData.indexBuffer = HardwareBufferManager.Singleton.CreateIndexBuffer(Mogre.HardwareIndexBuffer.IndexType.IT_16BIT, pSphereVertex.indexData.indexCount, HardwareBuffer.Usage.HBU_STATIC_WRITE_ONLY, false);
            HardwareIndexBufferSharedPtr iBuf = pSphereVertex.indexData.indexBuffer;
            ushort* pIndices = (ushort*)iBuf.Lock(HardwareBuffer.LockOptions.HBL_DISCARD);

            float fDeltaRingAngle = (float)(Mogre.Math.PI / nRings);
            float fDeltaSegAngle = (float)(2 * Mogre.Math.PI / nSegments);
            ushort wVerticeIndex = 0;

            // Generate the group of rings for the sphere
            for (int ring = 0; ring <= nRings; ring++) {
                float r0 = r * Mogre.Math.Sin(ring * fDeltaRingAngle);
                float y0 = r * Mogre.Math.Cos(ring * fDeltaRingAngle);

                // Generate the group of segments for the current ring
                for (int seg = 0; seg <= nSegments; seg++) {
                    float x0 = r0 * Mogre.Math.Sin(seg * fDeltaSegAngle);
                    float z0 = r0 * Mogre.Math.Cos(seg * fDeltaSegAngle);

                    // Add one vertex to the strip which makes up the sphere
                    *pVertex++ = x0;
                    *pVertex++ = y0;
                    *pVertex++ = z0;

                    Mogre.Vector3 vNormal = (new Mogre.Vector3(x0, y0, z0)).NormalisedCopy;
                    *pVertex++ = vNormal.x;
                    *pVertex++ = vNormal.y;
                    *pVertex++ = vNormal.z;

                    *pVertex++ = (float)seg / (float)nSegments;
                    *pVertex++ = (float)ring / (float)nRings;

                    if (ring != nRings) {
                        // each vertex (except the last) has six indices pointing to it
                        *pIndices++ = (ushort)(wVerticeIndex + nSegments + 1);
                        *pIndices++ = wVerticeIndex;
                        *pIndices++ = (ushort)(wVerticeIndex + nSegments);
                        *pIndices++ = (ushort)(wVerticeIndex + nSegments + 1);
                        *pIndices++ = (ushort)(wVerticeIndex + 1);
                        *pIndices++ = wVerticeIndex;
                        wVerticeIndex++;
                    }
                }; // end for seg
            } // end for ring

            // Unlock
            vBuf.Unlock();
            iBuf.Unlock();

            // Generate face list
            pSphereVertex.useSharedVertices = true;

            // the original code was missing this line:
            pSphere._setBounds(new AxisAlignedBox(new Mogre.Vector3(-r, -r, -r), new Mogre.Vector3(r, r, r)), false);
            pSphere._setBoundingSphereRadius(r);

            // this line makes clear the mesh is loaded (avoids memory leaks)
            pSphere.Load();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Remove()
        {
            if (!this.IsCreated)
            {
                return;
            }

            this.SceneNode.DetachAllObjects();
            this.SceneNode.ParentSceneNode.RemoveAndDestroyChild(this.SceneNode.Name);
            this.SceneNode = null;

            //Mogre.MeshManager.Singleton.Remove("SkyMesh");
            //this.SkyX.SceneManager.RemoveEntity(this.Entity);
            this.SkyX.SceneManager.DestroyEntity(this.Entity);
            Mogre.MeshManager.Singleton.Unload("SkyMesh");
            Mogre.MeshManager.Singleton.Remove("SkyMesh");
            this.Mesh = null;
            this.SubMesh = null;
            this.Entity = null;
            this.HardwareVertexBuffer = null;
            this.HardwareIndexBuffer = null;
            this.MaterialName = "_NULL_";
            this._vertices = null;
            this.IsCreated = false;
        }


        internal unsafe void  UpdateGeometry()
        {
            
            if (!this.IsCreated)
            {
                return;
            }
            // 95% of camera far clip distance
            float TODO_Radius = this.SkyX.Camera.FarClipDistance * 0.95f;

            _vertices[0].X = 0;
            _vertices[0].Z = 0;
            _vertices[0].Y = TODO_Radius;
            _vertices[0].NX = 0;
            _vertices[0].NZ = 0;
            _vertices[0].NY = 1;
            _vertices[0].U = 4;
            _vertices[0].V = 4;
            _vertices[0].Opacity = 1;

            float angleStep = (Utility.PI / 2.0f) / (this.Circles - 1.0f);

            float r, uvr, c, s, sc;
            int x, y;

            for (y = 0; y < this.Circles - 1; y++)
            {
                r = Utility.Cos(Utility.PI / 2.0f - angleStep * (y + 1.0f));
                uvr = (y + 1.0f) / (this.Circles - 1.0f);

                for (x = 0; x < this.Steps; x++)
                {
                    c = Utility.Cos(Utility.TWO_PI * x / (float)this.Steps) * r;
                    s = Utility.Sin(Utility.TWO_PI * x / (float)this.Steps) * r;
                    sc = Utility.Sin(Utility.ACos(r));

                    _vertices[1 + y * this.Steps + x].X = c * TODO_Radius;
                    _vertices[1 + y * this.Steps + x].Z = s * TODO_Radius;
                    _vertices[1 + y * this.Steps + x].Y = sc * TODO_Radius;

                    _vertices[1 + y * this.Steps + x].NX = c;
                    _vertices[1 + y * this.Steps + x].NZ = s;
                    _vertices[1 + y * this.Steps + x].NY = sc;

                    _vertices[1 + y * this.Steps + x].U = (1.0f + c * uvr / r) * 4.0f;
                    _vertices[1 + y * this.Steps + x].V = (1.0f + s * uvr / r) * 4.0f;

                    _vertices[1 + y * this.Steps + x].Opacity = 1;
                }
            }

            r = Utility.Cos(angleStep);
            uvr = (this.Circles + 1.0f) / (this.Circles - 1.0f);

            for (x = 0; x < this.Steps; x++)
            {
                c = Utility.Cos(Utility.TWO_PI * x / (float)this.Steps) * r;
                s = Utility.Sin(Utility.TWO_PI * x / (float)this.Steps) * r;
                _vertices[1 + (this.Circles - 1) * this.Steps + x].X = _vertices[1 + (this.Circles - 2) * this.Steps + x].X;
                _vertices[1 + (this.Circles - 1) * this.Steps + x].Z = _vertices[1 + (this.Circles - 2) * this.Steps + x].Z;
                _vertices[1 + (this.Circles - 1) * this.Steps + x].Y = _vertices[1 + (this.Circles - 2) * this.Steps + x].Y - TODO_Radius * this.SkydomeFadingPercent;

                _vertices[1 + (this.Circles - 1) * this.Steps + x].NX = _vertices[1 + (this.Circles - 2) * this.Steps + x].NX;
                _vertices[1 + (this.Circles - 1) * this.Steps + x].NZ = _vertices[1 + (this.Circles - 2) * this.Steps + x].NZ;
                _vertices[1 + (this.Circles - 1) * this.Steps + x].NY = _vertices[1 + (this.Circles - 2) * this.Steps + x].NY;

                _vertices[1 + (this.Circles - 1) * this.Steps + x].U = (1.0f + c * uvr / 4) * 4.0f;
                _vertices[1 + (this.Circles - 1) * this.Steps + x].V = (1.0f + s * uvr / 4) * 4.0f;

                _vertices[1 + (this.Circles - 1) * this.Steps + x].Opacity = this.SmoothSkydomeFading ? 0 : 1;
            }
            // Update data
            fixed (PosUVVertex* addr = &_vertices[0]) {
                _vertexBuffer.WriteData(0, _vertexBuffer.SizeInBytes, addr, true);
            }
            // Update bounds
            AxisAlignedBox meshBounds = new AxisAlignedBox(new Vector3(-TODO_Radius, 0, -TODO_Radius),
                                                            new Vector3(TODO_Radius, TODO_Radius, TODO_Radius));

            //_mesh.BoundingBox = meshBounds;
            _mesh._setBounds(meshBounds);
            _sceneNode.NeedUpdate();
            //for (int i = 0; i < _vertices.Length; i++)
            //    LogVertices(_vertices[i]);
        }
        void LogVertices(PosUVVertex vertices)
        {
            LogManager.Singleton.LogMessage(
                "NX " + vertices.NX + " " +
                "NY " + vertices.NY + " " +
                "NZ " + vertices.NZ + " " +
                "O " + vertices.Opacity + " " +
                "U " + vertices.U + " " +
                "V " + vertices.V + " " +
                "X " + vertices.X + " " +
                "Y " + vertices.Y + " " +
                "Z " + vertices.Z);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="smoothSkyDomeFading"></param>
        public void SetSkydomeFadingParameters(bool smoothSkyDomeFading)
        {
            SetSkydomeFadingParameters(smoothSkyDomeFading, 0.05f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="smoothSkydomeFading"></param>
        /// <param name="skydomeFadingPercent"></param>
        public void SetSkydomeFadingParameters(bool smoothSkydomeFading, float skydomeFadingPercent)
        {
            this.SmoothSkydomeFading = smoothSkydomeFading;
            this.SkydomeFadingPercent = skydomeFadingPercent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        private void SetMaterialName(string name)
        {
            _materialName = name;

            if (this.IsCreated)
            {
                this.Entity.SetMaterialName(this.MaterialName);
                //this.Entity.MaterialName = this.MaterialName;
            }
        }

        internal void SetGeometryParameters(int steps, int circles)
        {
            this.Steps = steps;
            this.Circles = circles;

            if (this.IsCreated)
            {
                Remove();
                Create();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private unsafe void CreateGeometry()
        {
            int numVertices = this.Steps * this.Circles + 1;
            int numEule = 6 * this.Steps * (this.Circles - 1) + 3 * this.Steps;

            // Vertex buffers
            this.SubMesh.vertexData = new VertexData();
            this.SubMesh.vertexData.vertexStart = 0;
            this.SubMesh.vertexData.vertexCount = (uint)numVertices;

            VertexDeclaration vdecl = this.SubMesh.vertexData.vertexDeclaration;
            VertexBufferBinding vbind = this.SubMesh.vertexData.vertexBufferBinding;

            uint offset = 0;
            vdecl.AddElement(0, offset, VertexElementType.VET_FLOAT3, VertexElementSemantic.VES_POSITION);
            offset += VertexElement.GetTypeSize(VertexElementType.VET_FLOAT3);
            vdecl.AddElement(0, offset, VertexElementType.VET_FLOAT3, VertexElementSemantic.VES_TEXTURE_COORDINATES, 0);
            offset += VertexElement.GetTypeSize(VertexElementType.VET_FLOAT3);
            vdecl.AddElement(0, offset, VertexElementType.VET_FLOAT2, VertexElementSemantic.VES_TEXTURE_COORDINATES, 1);
            offset += VertexElement.GetTypeSize(VertexElementType.VET_FLOAT2);
            vdecl.AddElement(0, offset, VertexElementType.VET_FLOAT1, VertexElementSemantic.VES_TEXTURE_COORDINATES, 2);
            offset += VertexElement.GetTypeSize(VertexElementType.VET_FLOAT1);

            _vertexBuffer = HardwareBufferManager.Singleton.CreateVertexBuffer(offset, (uint)numVertices, HardwareBuffer.Usage.HBU_DYNAMIC_WRITE_ONLY);

            vbind.SetBinding(0, _vertexBuffer);

            int[] indexBuffer = new int[numEule];
            for (int k = 0; k < this.Steps; k++)
            {
                indexBuffer[k * 3] = 0;
                indexBuffer[k * 3 + 1] = k + 1;

                if (k != this.Steps - 1)
                {
                    indexBuffer[k * 3 + 2] = k + 2;
                }
                else
                {
                    indexBuffer[k * 3 + 2] = 1;
                }
            }

            for (int y = 0; y < this.Circles - 1; y++)
            {
                for (int x = 0; x < this.Steps; x++)
                {

                    int twoface = (y * this.Steps + x) * 6 + 3 * this.Steps;

                    int p0 = 1 + y * this.Steps + x;
                    int p1 = 1 + y * this.Steps + x + 1;
                    int p2 = 1 + (y + 1) * this.Steps + x;
                    int p3 = 1 + (y + 1) * this.Steps + x + 1;

                    if (x == this.Steps - 1)
                    {
                        p1 -= x + 1;
                        p3 -= x + 1;
                    }

                    // First triangle
                    indexBuffer[twoface + 2] = p0;
                    indexBuffer[twoface + 1] = p1;
                    indexBuffer[twoface + 0] = p2;

                    // Second triangle
                    indexBuffer[twoface + 5] = p1;
                    indexBuffer[twoface + 4] = p3;
                    indexBuffer[twoface + 3] = p2;
                }

            }
            // Prepare buffer for indices
            _indexBuffer = HardwareBufferManager.Singleton.CreateIndexBuffer(Mogre.HardwareIndexBuffer.IndexType.IT_32BIT, (uint)numEule, HardwareBuffer.Usage.HBU_STATIC, true);
            //for(int z = 0; z < indexBuffer.Length;z++)
            //    LogManager.Singleton.Write("Index " + indexBuffer[z]);
            fixed (int* addr = &indexBuffer[0]) {
                _indexBuffer.WriteData(0, _indexBuffer.SizeInBytes, addr, true);
            }
            // Set index buffer for this submesh
            this.SubMesh.indexData.indexBuffer = _indexBuffer;
            this.SubMesh.indexData.indexStart = 0;
            this.SubMesh.indexData.indexCount = (uint)numEule;

            // Create our internal buffer for manipulations
            _vertices = new PosUVVertex[1 + this.Steps * this.Circles];
        }
    }
}