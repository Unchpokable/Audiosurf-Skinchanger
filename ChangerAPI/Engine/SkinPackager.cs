﻿namespace ChangerAPI.Engine
{
    using ChangerAPI.Utilities;
    using System;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    public class SkinPackager
    {
        public static readonly string skinExtension = @".askin";
        public string OutputPath { get; set; }
        //private Logger logger;
        private string[] texturesNames;
        private string[] masks;
        private readonly string defaultOutput = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public SkinPackager()
        {
            //logger = new Logger();
            texturesNames = new[]
            {
                "cliff-1.png", "cliff-2.png", "cliff2-1.png", "cliff2-1.png", "hit1.png", "hit2.png",
                "particles1.png", "particles2.png", "particles3.png", "ring1A.png", "ring1B.png", "ring2A.jpg", "ring2B",
                "Skyshpere_Black.png", "Skysphere_Grey.png", "Skyshphere_White.png",
                "tileflyup.png", "tiles.png"
            };

            masks = new[]
            {
                EnvironmentalVeriables.CliffImagesMask,
                EnvironmentalVeriables.HitImageMask,
                EnvironmentalVeriables.ParticlesImageMask,
                EnvironmentalVeriables.RingsImageMask,
                EnvironmentalVeriables.SkysphereImagesMask,
            };
        }

        public bool Compile(AudiosurfSkin skin)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                using (Stream filestream = new FileStream((OutputPath ?? defaultOutput) + @"\\" + skin.Name + skinExtension, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(filestream, skin);
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool CompileTo(AudiosurfSkin skin, string path)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                using (Stream filestream = new FileStream(path + @"\\" + skin.Name + skinExtension, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(filestream, skin);
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool RewriteCompile(AudiosurfSkin skin, string path)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                using (Stream filestream = new FileStream(path, FileMode.Create))
                {
                    formatter.Serialize(filestream, skin);
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public AudiosurfSkin Decompile(string path)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                using (Stream skinFileStream = new FileStream(path, FileMode.Open))
                    return (AudiosurfSkin)formatter.Deserialize(skinFileStream);
            }

            catch (IOException e)
            {
                return null;
            }

            catch (Exception e)
            {
                return null;
            }
        }

        public AudiosurfSkin CreateSkinFromFolder(string path)
        {
            var result = new AudiosurfSkin();
            
            string[] AllPictures = Directory.GetFiles(path);
            if (!AllPictures.Any(fileName => texturesNames.Contains(Path.GetFileName(fileName))))
                return null;

            foreach(var mask in masks)
            {
                if (mask == EnvironmentalVeriables.CliffImagesMask)
                    result.Cliffs = GetAllImagesByNameMask("cliffs", mask, path);
                if (mask == EnvironmentalVeriables.HitImageMask)
                    result.Hits = GetAllImagesByNameMask("hits", mask, path);
                if (mask == EnvironmentalVeriables.ParticlesImageMask)
                    result.Particles = GetAllImagesByNameMask("particles", mask, path);
                if (mask == EnvironmentalVeriables.RingsImageMask)
                    result.Rings = GetAllImagesByNameMask("rings", mask, path);
                if (mask == EnvironmentalVeriables.SkysphereImagesMask)
                    result.SkySpheres = GetAllImagesByNameMask("skysphere", mask, path);
            }

            ImageGroup tiles = GetAllImagesByNameMask("tiles", "tiles.png", path);
            ImageGroup tileflyup = GetAllImagesByNameMask("tiles flyup", "tileflyup.png", path);

            if (tiles.Group.Count > 1 || tileflyup.Group.Count > 1)
            {
                return null;
            }

            result.Tiles = new NamedBitmap("tiles.png", (Bitmap)tiles);
            result.TilesFlyup = new NamedBitmap("tileflyup.png", (Bitmap)tileflyup);
            return result;
        }

        private ImageGroup GetAllImagesByNameMask(string groupName, string nameMask, string path)
        {
            var group = new ImageGroup(groupName);

            string[] AllFiles = Directory.GetFiles(path);

            foreach(var file in AllFiles)
            {
                var origName = Path.GetFileName(file);
                var fname = origName.ToLower();
                var fileExt = Path.GetExtension(fname);
                if (fname.Contains(nameMask) && new[] { ".png", ".jpg" }.Any(x => x == fileExt))
                {
                    var image = new NamedBitmap(origName, Image.FromFile(file));
                    group.AddImage(image);
                }
            }

            return group;
        }
    }
}