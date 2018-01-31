using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace GameStateManagement
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Actor : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Fields

        protected Model modelMesh;
        public Matrix worldMatrix;

        public Boolean looped = false;
        public Boolean canFire = true;
        protected float fMass;
        protected float fTerminalVelocity = .5f;
        protected Vector3 vForce;
        private Vector3 vAcceleration;
        protected bool bPhysicsDriven = false;
        public Boolean paused = false;
        public Boolean lighting = true;

        private float worldScale;
        private Vector3 worldPosition;
        private Vector3 oldWorldPosition;
        private Quaternion worldRotation;

        public BoundingSphere modelBounds;
        public BoundingSphere worldBounds;
        public BoundingBox boundingBox;

        public float Scale
        {
            set
            {
                worldScale = value;
                worldMatrix = Matrix.CreateScale(worldScale) * Matrix.CreateFromQuaternion(worldRotation) * Matrix.CreateTranslation(worldPosition);
            }
        }
        
        public Quaternion Rotation
        {
            set
            {
                worldRotation = value;
                worldMatrix = Matrix.CreateScale(worldScale) * Matrix.CreateFromQuaternion(worldRotation) * Matrix.CreateTranslation(worldPosition);
            }

            get
            {
                return worldRotation;
            }
        }
        public Vector3 Position
        {
            set
            {
                if (value.X < 2000 && value.X > -2000 && value.Y < 2400 && value.Y > -2400)
                {
                    worldBounds.Center = value;
                    worldBounds.Radius = modelBounds.Radius * worldScale * 0.8f;
                    oldWorldPosition = worldPosition;
                    worldPosition = value;
                    worldMatrix = Matrix.CreateScale(worldScale) * Matrix.CreateFromQuaternion(worldRotation) * Matrix.CreateTranslation(worldPosition);
                }
                else
                {
                    looped = true;
                }
            }

            get
            {
                return worldPosition;
            }
        }
        public Vector3 velocity;

        //private ContentManager cManager;
        protected static string meshName;
        private Utils.Timer timer;
        private Matrix[] boneTransforms;

        #endregion

        public Actor(Game game)
            : base(game)
        {
            timer = new Utils.Timer();
            velocity = Vector3.Zero;

            worldScale = 1.0f;
            worldPosition = Vector3.Zero;
            worldRotation = Quaternion.Identity;
            worldMatrix = Matrix.Identity;

        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            //boundingBox = UpdateBoundingBox(modelMesh, worldMatrix);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (!paused)
            {
                //worldBounds

                if (bPhysicsDriven)
                {
                    velocity += vAcceleration * (float)gameTime.ElapsedGameTime.TotalSeconds / 2.0f;
                    Position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    vAcceleration = vForce / fMass;
                    velocity += vAcceleration * (float)gameTime.ElapsedGameTime.TotalSeconds / 2.0f;

                    if (velocity.Length() > fTerminalVelocity)
                    {
                        velocity = Vector3.Normalize(velocity) * fTerminalVelocity;
                    }
                }
                else
                {
                    Position = (velocity * (float)gameTime.ElapsedGameTime.TotalSeconds * 1) + worldPosition;
                    //Position = (velocity * 0.01) + worldPosition;
                }

                timer.Update(gameTime);
                base.Update(gameTime);
            }
        }

        protected override void LoadContent()
        {
            //cManager = Game.Content.Load<(Game.Services, "Content");
            modelMesh = Game.Content.Load<Model>("Models/" + meshName);
            boneTransforms = new Matrix[modelMesh.Bones.Count];

            foreach (ModelMesh mesh in modelMesh.Meshes)
            {
                modelBounds = BoundingSphere.CreateMerged(modelBounds, mesh.BoundingSphere);
            }
            boundingBox = UpdateBoundingBox(modelMesh, worldMatrix);

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            Game.Content.Unload();
            //boundingBox = null;
            base.UnloadContent();
        }

         public override void Draw(GameTime gameTime)
        {
            if (!paused)
            {
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
                modelMesh.CopyAbsoluteBoneTransformsTo(boneTransforms);
                foreach (ModelMesh mesh in modelMesh.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = boneTransforms[mesh.ParentBone.Index] * worldMatrix;
                        effect.View = GameplayScreen.cameraMatrix;
                        effect.Projection = GameplayScreen.projectionMatrix;

                        if (lighting)
                        {
                            effect.EnableDefaultLighting();
                            effect.PreferPerPixelLighting = true;
                            effect.AmbientLightColor = GameplayScreen.ambientLight;
                            effect.SpecularColor = GameplayScreen.specularColor;
                            effect.SpecularPower = GameplayScreen.specularPower;
                            effect.DirectionalLight0.Direction = GameplayScreen.lightDirection;
                            effect.DirectionalLight0.DiffuseColor = GameplayScreen.diffuseColor;
                        }
                        
                    }
                    mesh.Draw();
                }
            }

            //base.Draw(gameTime);
        }

        protected BoundingBox UpdateBoundingBox(Model model, Matrix worldTransform)
        {
            // Initialize minimum and maximum corners of the bounding box to max and min values
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            // For each mesh of the model
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Vertex buffer parameters
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    // Get vertex data as float
                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    // Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), worldTransform);

                        min = Vector3.Min(min, transformedPosition);
                        max = Vector3.Max(max, transformedPosition);
                    }
                }
            }

            // Create and return bounding box
            return new BoundingBox(min, max);
        }


        public Vector3 GetWorldFacing()
        {
            return worldMatrix.Forward;
        }

        public Vector3 GetWorldPosition()
        {
            return worldMatrix.Translation;
        }

        public Vector3 GetWorldPosition2()
        {
            return worldPosition;
        }

        public void SetWorldPosition(float x, float y, float z)
        {
            worldPosition.X = x;
            worldPosition.Y = y;
            worldPosition.Z = z;
        }

        public Vector3 GetOldWorldPosition()
        {
            return oldWorldPosition;
        }

        public void createRotation(Vector3 axis, float angle)
        {
            Quaternion mRotation;
            Vector3 mAxis = Vector3.Normalize(axis);
            Quaternion.CreateFromAxisAngle(ref mAxis, angle, out mRotation);
            Rotation = Quaternion.Concatenate(Rotation, mRotation);
        }

        public void SnapRotate(float angle)
        {
            Vector3 mAxis = Vector3.Normalize(new Vector3(0.0f, 1.0f, 0.0f));
            Rotation = Quaternion.CreateFromAxisAngle(mAxis, angle - MathHelper.Pi);
            createRotation(new Vector3(1f, 0f, 0f), MathHelper.PiOver2);

        }

        public virtual void setPaused(Boolean b)
        {
            paused = b;
        }

        /*public BoundingBox CreateBoundingBox(Model model)
        {
            BoundingBox boundingBox = new BoundingBox();
            BoundingBox.CreateFromPoints(model.Meshes

            foreach (ModelMesh mesh in modelMesh.Meshes)
            {
                VertexPositionNormalTexture[] vertices =
                new VertexPositionNormalTexture[mesh.VertexBuffer.SizeInBytes / VertexPositionNormalTexture.SizeInBytes];

                mesh.VertexBuffer.GetData<VertexPositionNormalTexture>(vertices);

                Vector3[] vertexs = new Vector3[vertices.Length];

                for (int index = 0; index < vertexs.Length; index++)
                {
                    vertexs[index] = vertices[index].Position;
                }

                boundingBox = BoundingBox.CreateMerged(boundingBox,
                BoundingBox.CreateFromPoints(vertexs));
            }

            return boundingBox;
        }*/
    }
}
