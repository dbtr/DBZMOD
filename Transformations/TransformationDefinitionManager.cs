﻿using System.Collections.Generic;
using DBZMOD.Buffs;
using DBZMOD.Dynamicity;
using DBZMOD.Enums;
using DBZMOD.Managers;
using DBZMOD.Utilities;
using Microsoft.Xna.Framework;

namespace DBZMOD.Transformations
{
    public class TransformationDefinitionManager : Manager<TransformationDefinition>
    {
        public static readonly Color 
            defaultTransformationTextColor = new Color(219, 219, 48),
            godTransformationTextColor = new Color(229, 20, 51);

        internal override void DefaultInitialize()
        {
            Add(KaiokenDefinition = new TransformationDefinition(MenuSelectionID.None, BuffKeyNames.kaioken, null, defaultTransformationTextColor));
            Add(SuperKaiokenDefinition = new TransformationDefinition(MenuSelectionID.None, BuffKeyNames.superKaioken, null, defaultTransformationTextColor, true, BuffKeyNames.ssj1));
            Add(KaiokenFatigueDefinition = new TransformationDefinition(MenuSelectionID.None, BuffKeyNames.kaiokenFatigue, null, defaultTransformationTextColor));

            Add(SSJ1Definition = new TransformationDefinition(MenuSelectionID.SSJ1, BuffKeyNames.ssj1, "Super Saiyan 1", defaultTransformationTextColor, true, BuffKeyNames.ssj1));
            Add(ASSJDefinition = new TransformationDefinition(MenuSelectionID.None, BuffKeyNames.assj, FormBuffHelper.GetASSJNamePreference(), defaultTransformationTextColor, true, BuffKeyNames.ssj1, null, SSJ1Definition));
            Add(USSJDefinition = new TransformationDefinition(MenuSelectionID.None, BuffKeyNames.ussj, FormBuffHelper.GetUSSJNamePreference(), defaultTransformationTextColor, true, BuffKeyNames.ssj1, null, ASSJDefinition));

            Add(SSJ2Definition = new TransformationDefinition(MenuSelectionID.SSJ2, BuffKeyNames.ssj2, "Super Saiyan 2", defaultTransformationTextColor, true, BuffKeyNames.ssj2, null, SSJ1Definition));
            Add(SSJ3Definition = new TransformationDefinition(MenuSelectionID.SSJ3, BuffKeyNames.ssj3, "Super Saiyan 3", defaultTransformationTextColor, true, BuffKeyNames.ssj3, null, SSJ2Definition));

            Add(SSJGDefinition = new TransformationDefinition(MenuSelectionID.SSJG, BuffKeyNames.ssjg, "Super Saiyan God", godTransformationTextColor, true, BuffKeyNames.ssjg, null, SSJ1Definition));
            Add(SSJBDefinition = new TransformationDefinition(MenuSelectionID.Ssjb, BuffKeyNames.ssjb, null, defaultTransformationTextColor, true, BuffKeyNames.ssjb, null, SSJ1Definition));

            Add(LSSJDefinition = new TransformationDefinition(MenuSelectionID.LSSJ1, BuffKeyNames.lssj, "Legendary Super Saiyan", defaultTransformationTextColor, true, BuffKeyNames.lssj));
            Add(LSSJ2Definition = new TransformationDefinition(MenuSelectionID.LSSJ2, BuffKeyNames.lssj2, "Legendary Super Saiyan 2", defaultTransformationTextColor, true, BuffKeyNames.lssj2, null, LSSJDefinition));

            Add(TransformationExhaustionDefinition = new TransformationDefinition(MenuSelectionID.None, BuffKeyNames.transformationExhaustion, null, defaultTransformationTextColor));
            Add(SpectrumDefinition = new TransformationDefinition(MenuSelectionID.Spectrum, BuffKeyNames.spectrum, "Super Saiyan Spectrum", defaultTransformationTextColor, true, BuffKeyNames.spectrum));
        }

        public Dictionary<TransformationDefinition, ManyToManyNode<TransformationDefinition>> BuildTransformationTree()
        {
            Dictionary<TransformationDefinition, ManyToManyNode<TransformationDefinition>> tree = new Dictionary<TransformationDefinition, ManyToManyNode<TransformationDefinition>>();

            foreach (TransformationDefinition def in byIndex)
            {
                if (!tree.ContainsKey(def))
                    tree.Add(def, new ManyToManyNode<TransformationDefinition>());

                tree[def].AddPrevious(def.PreviousTransformations);
                tree[def].Current = def;

                for (int i = 0; i < def.PreviousTransformations.Length; i++)
                    tree[def.PreviousTransformations[i]].AddNext(def);
            }

            return tree;
        }

        public TransformationDefinition KaiokenDefinition { get; private set; }
        public TransformationDefinition SuperKaiokenDefinition { get; private set; }
        public TransformationDefinition KaiokenFatigueDefinition { get; private set; }

        public TransformationDefinition SSJ1Definition { get; private set; } 
        public TransformationDefinition ASSJDefinition { get; private set; }
        public TransformationDefinition USSJDefinition { get; private set; }

        public TransformationDefinition SSJ2Definition { get; private set; }
        public TransformationDefinition SSJ3Definition { get; private set; }

        public TransformationDefinition SSJGDefinition { get; private set; }
        public TransformationDefinition SSJBDefinition { get; private set; }

        public TransformationDefinition LSSJDefinition { get; private set; }
        public TransformationDefinition LSSJ2Definition { get; private set; }

        public TransformationDefinition TransformationExhaustionDefinition { get; private set; }
        public TransformationDefinition SpectrumDefinition { get; private set; }
    }
}
