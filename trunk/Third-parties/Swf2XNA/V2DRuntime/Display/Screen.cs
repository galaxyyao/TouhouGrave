using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DDW.V2D;
using DDW.V2D.Serialization;
using Microsoft.Xna.Framework.Content;
using System.IO;
using V2DRuntime.Shaders;
using System.Reflection;
using V2DRuntime.V2D;

namespace DDW.Display
{
	public delegate void ShaderEffect(Effect effect, int firstPass, params float[] parameters);

    public class Screen : DisplayObjectContainer
    {
        public V2DWorld v2dWorld;
        public Dictionary<string, Texture2D> textures = new Dictionary<string,Texture2D>();
        private SymbolImport symbolImport;

		public Vector2 ClientSize = new Vector2(400, 300);

		protected int framesBetweenPackets = 4;
		protected int framesSinceLastSend;
		private List<DisplayObject> destructionList = new List<DisplayObject>();
        public Dictionary<int, V2DShader> shaderMap = new Dictionary<int, V2DShader>();

        public bool isFinalLevel = false;
        public bool isPersistantScreen = false;

        public Screen()
        {
            SetAttributes();
        }
        public Screen(SymbolImport symbolImport)
        {
            this.SymbolImport = symbolImport;

			EnsureV2DWorld();
			if (SymbolImport == null || SymbolImport.instanceName == CurrentRootName)
			{
				instanceDefinition = v2dWorld.RootInstance;
				instanceName = ROOT_NAME;
			}
			else
			{
				instanceDefinition = FindRootInstance(v2dWorld.RootInstance, SymbolImport.instanceName);
				instanceName = symbolImport.instanceName;
			}

			if (instanceDefinition != null)
			{
				definitionName = instanceDefinition.DefinitionName;
            }
            SetAttributes();
        }
        public Screen(V2DContent v2dContent)
        {
            this.v2dWorld = v2dContent.v2dWorld;
            this.textures = v2dContent.textures;
            SetAttributes();
        }

        private void SetAttributes()
        {
            System.Reflection.MemberInfo inf = this.GetType();
            System.Attribute[] attrs = System.Attribute.GetCustomAttributes(inf);  // reflection
            foreach (System.Attribute attr in attrs)
            {
                if (attr is ScreenAttribute)
                {
                    ScreenAttribute sa = (ScreenAttribute)attr;
                    if (sa.backgroundColor != 0x000000)
                    {
                        this.color = new Color(
                            ((sa.backgroundColor & 0xFF0000) >> 16) / 255f,
                            ((sa.backgroundColor & 0x00FF00) >> 8) / 255f,
                            ((sa.backgroundColor & 0x0000FF) >> 0) / 255f);
                    }

                    if (sa.depthGroup != 0)
                    {
                        DepthGroup = sa.depthGroup;
                    }

                    if (sa.isPersistantScreen != false)
                    {
                        isPersistantScreen = sa.isPersistantScreen;
                    }
                }
                if (attr is V2DShaderAttribute)
                {
                    V2DShaderAttribute sa = (V2DShaderAttribute)attr;

                    float[] parameters = new float[] { };
                    ConstructorInfo ci = sa.shaderType.GetConstructor(new Type[] { parameters.GetType() });
                    this.defaultShader = (V2DShader)ci.Invoke(
                        new object[] 
							{ 
								new float[]{sa.param0, sa.param1, sa.param2, sa.param3, sa.param4} 
							});
                }
            }
        }
		public void DestroyAfterUpdate(DisplayObject obj)
		{
			destructionList.Add(obj);
		}

		public override void Activate()
		{
			CurrentRootName = instanceDefinition.InstanceName == null ? ROOT_NAME : instanceDefinition.InstanceName;
            base.Activate();
		}

		public override void Added(EventArgs e)
		{
            base.Added(e); 

			//V2DGame.instance.SetSize(v2dWorld.Width, v2dWorld.Height);
			Activate();
		}
		public override void Removed(EventArgs e)
		{
			base.Removed(e);
			Deactivate();
		}

        public SymbolImport SymbolImport
        {
            get
            {
                return symbolImport;
            }
            set
            {
                symbolImport = value;
            }
        }
		public virtual Sprite CreateDefaultObject(Texture2D texture, V2DInstance inst)
		{
			return new Sprite(texture, inst);
		}
        public Texture2D GetTexture(string linkageName)
        {
            Texture2D result = null;
            if (this.textures.ContainsKey(linkageName))
            {
                result = this.textures[linkageName];
            }
            else
            {
				this.textures[linkageName] = null;
				//string fullPath = Path.Combine(
				//   StorageContainer.TitleLocation,
				//   Path.Combine(V2DGame.contentManager.RootDirectory, linkageName) + ".png");

				//if (File.Exists(fullPath))
				//{
				//    result = V2DGame.contentManager.Load<Texture2D>(linkageName);
				//}
				//else
				//{
				//    try
				//    {
				//        result = V2DGame.contentManager.Load<Texture2D>(linkageName);
				//    }
				//    catch(ContentLoadException)
				//    {
				//        this.textures[linkageName] = null;
				//    }
				//}
            }

            return result;
        }

        V2DContent content;
		private void EnsureV2DWorld()
		{
			if (SymbolImport != null && v2dWorld == null)
			{
                // ** note: unnamed elements may actually fall out of scope and get gc/disposed, so need a ref
                // todo: use multiple content loaders per screen and unload where needed.
                //V2DContent content = V2DGame.instance.Content.Load<V2DContent>(SymbolImport.assetName);
                content = V2DRuntime.ResourceManager.Instance.LoadV2DContent(SymbolImport.assetName);
                v2dWorld = content.v2dWorld;
                textures = content.textures;
				v2dWorld.RootInstance.Definition = v2dWorld.GetDefinitionByName(ROOT_NAME);
			}
		}

		private V2DInstance FindRootInstance(V2DInstance inst, string rootName)
		{
			// look through higher level instances first
			V2DInstance result = null;

			if (inst.InstanceName == rootName)
			{
				result = inst;
			}
			else if (inst.Definition != null)
			{
				for (int i = 0; i < inst.Definition.Instances.Count; i++)
				{
					if (inst.Definition.Instances[i].InstanceName == rootName)
					{
						result = inst.Definition.Instances[i];
						break;
					}
				}
			}

			if (result == null && inst.Definition != null)
			{
				for (int i = 0; i < inst.Definition.Instances.Count; i++)
				{
					result = FindRootInstance(inst.Definition.Instances[i], rootName);
					if (result != null)
					{
						break;
					}
				}
			}

            if (result == null)
            {
                throw new Exception("Could not find root instance: " + rootName + " in " + inst.InstanceName + ".");
            }
			return result;
		}

        public virtual void SetBounds(float x, float y, float w, float h)
		{
		}
		public override void Update(float deltaTimeMs)
        {
            base.Update(deltaTimeMs);

            OnUpdateComplete();		
		}

		public virtual void OnUpdateComplete()
		{
			if (destructionList.Count > 0)
			{
				foreach (DisplayObject ds in destructionList)
				{
					DestroyElement(ds);
				}
				destructionList.Clear();
			}	
		}
        public override void Draw(SpriteBatch batch)
        {
            if (defaultShader != null)
            {
                defaultShader.Begin(batch);
            }
            else
            {
                batch.Begin(
                   SpriteSortMode.Deferred,
                   BlendState.AlphaBlend, //BlendState.NonPremultiplied,
                   null, //SamplerState.AnisotropicClamp, 
                   null, //DepthStencilState.None, 
                   null, //RasterizerState.CullNone, 
                   null,
                   DisplayObject.GlobalTransform);
            }

            base.Draw(batch);

            // this needs to happen for screen (class) level shaders (vs depth group shaders)
            if (lastShader != null)
            {
                lastShader.End(batch);
                lastShader = null;
            }
            else if (defaultShader != null)
            {
                defaultShader.End(batch);
            }
            else
            {
                batch.End();
            }
        }
		public virtual void DrawDebugData(SpriteBatch batch)
		{
		}

		//Stack<V2DShader> shaderStack = new Stack<V2DShader>();
		public V2DShader lastShader;
		public V2DShader defaultShader;
		public V2DShader currentShader;
		protected override void DrawChild(DisplayObject d, SpriteBatch batch)
		{
            var shaderEffect = shaderMap.ContainsKey(d.DepthGroup) ? shaderMap[d.DepthGroup] : defaultShader;

            if (shaderEffect != lastShader)
            {
                //if (lastShader != null)
                //{
                //    lastShader.End(batch);
                //}

                batch.End();

                lastShader = shaderEffect;
                if (shaderEffect != null)
                {
                    shaderEffect.Begin(batch);
                    shaderEffect = null;
                }
                else
                {               
                    batch.Begin(
                         SpriteSortMode.Deferred,
                         BlendState.AlphaBlend, //BlendState.NonPremultiplied,
                         null, //SamplerState.AnisotropicClamp, 
                         null, //DepthStencilState.None, 
                         null, //RasterizerState.CullNone, 
                         null,
                         DisplayObject.GlobalTransform);
                }
            }

            base.DrawChild(d, batch);
        }

    }
}
