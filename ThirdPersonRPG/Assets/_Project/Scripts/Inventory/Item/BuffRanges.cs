using System.Collections.Generic;

namespace LB.Inventory
{
	public static class BuffRanges
	{
		public static Dictionary<ItemTier, Dictionary<Attributes, (int Min, int Max)>> MeleeWeaponBuffs = new()
		{
			{ ItemTier.Common, new Dictionary<Attributes, (int Min, int Max)> { { Attributes.Strength, (5, 10) } } },
			{ ItemTier.Uncommon, new Dictionary<Attributes, (int Min, int Max)> { { Attributes.Strength, (10, 20) } } },
			{
				ItemTier.Rare,
				new Dictionary<Attributes, (int Min, int Max)>
					{ { Attributes.Strength, (20, 30) }, { Attributes.CriticalHitChance, (2, 4) } }
			},
			{
				ItemTier.Epic,
				new Dictionary<Attributes, (int Min, int Max)>
					{ { Attributes.Strength, (30, 40) }, { Attributes.CriticalHitChance, (4, 8) } }
			},
			{
				ItemTier.Legendary,
				new Dictionary<Attributes, (int Min, int Max)>
				{
					{ Attributes.Strength, (40, 60) }, { Attributes.CriticalHitChance, (8, 16) },
					{ Attributes.Dexterity, (5, 10) }
				}
			},
			{
				ItemTier.Mythical,
				new Dictionary<Attributes, (int Min, int Max)>
				{
					{ Attributes.Strength, (60, 80) }, { Attributes.CriticalHitChance, (16, 20) },
					{ Attributes.Dexterity, (10, 20) }, { Attributes.Mana, (5, 10) }
				}
			}
		};

		public static Dictionary<ItemTier, Dictionary<Attributes, (int Min, int Max)>> ShieldBuffs = new()
		{
			{ ItemTier.Common, new Dictionary<Attributes, (int Min, int Max)> { { Attributes.Armor, (5, 10) } } },
			{ ItemTier.Uncommon, new Dictionary<Attributes, (int Min, int Max)> { { Attributes.Armor, (10, 20) } } },
			{
				ItemTier.Rare,
				new Dictionary<Attributes, (int Min, int Max)>
					{ { Attributes.Armor, (20, 30) }, { Attributes.Health, (5, 10) } }
			},
			{
				ItemTier.Epic,
				new Dictionary<Attributes, (int Min, int Max)>
				{
					{ Attributes.Armor, (30, 40) }, { Attributes.Health, (10, 20) },
					{ Attributes.RegenerationHealth, (2, 4) }
				}
			},
			{
				ItemTier.Legendary,
				new Dictionary<Attributes, (int Min, int Max)>
				{
					{ Attributes.Armor, (40, 60) }, { Attributes.Health, (20, 30) },
					{ Attributes.RegenerationHealth, (4, 8) }, { Attributes.Evasion, (2, 4) }
				}
			},
			{
				ItemTier.Mythical,
				new Dictionary<Attributes, (int Min, int Max)>
				{
					{ Attributes.Armor, (60, 80) }, { Attributes.Health, (30, 50) },
					{ Attributes.RegenerationHealth, (8, 12) }, { Attributes.Evasion, (4, 6) },
					{ Attributes.Luck, (2, 4) }
				}
			}
		};

		public static Dictionary<ItemTier, Dictionary<Attributes, (int Min, int Max)>> HelmetBuffs = new()
		{
			{ ItemTier.Common, new Dictionary<Attributes, (int Min, int Max)> { { Attributes.Armor, (3, 7) } } },
			{
				ItemTier.Uncommon,
				new Dictionary<Attributes, (int Min, int Max)>
					{ { Attributes.Armor, (7, 13) }, { Attributes.Intellect, (1, 3) } }
			},
			{
				ItemTier.Rare,
				new Dictionary<Attributes, (int Min, int Max)>
					{ { Attributes.Armor, (13, 17) }, { Attributes.Intellect, (3, 7) }, { Attributes.Health, (3, 7) } }
			},
			{
				ItemTier.Epic,
				new Dictionary<Attributes, (int Min, int Max)>
				{
					{ Attributes.Armor, (17, 23) }, { Attributes.Intellect, (7, 13) }, { Attributes.Health, (7, 13) }
				}
			},
			{
				ItemTier.Legendary,
				new Dictionary<Attributes, (int Min, int Max)>
				{
					{ Attributes.Armor, (23, 37) }, { Attributes.Intellect, (13, 17) }, { Attributes.Health, (13, 27) },
					{ Attributes.RegenerationHealth, (3, 7) }
				}
			},
			{
				ItemTier.Mythical,
				new Dictionary<Attributes, (int Min, int Max)>
				{
					{ Attributes.Armor, (37, 63) }, { Attributes.Intellect, (17, 33) }, { Attributes.Health, (27, 33) },
					{ Attributes.RegenerationHealth, (7, 13) }
				}
			}
		};

		public static Dictionary<ItemTier, Dictionary<Attributes, (int Min, int Max)>> ChestBuffs = new()
		{
			{ ItemTier.Common, new Dictionary<Attributes, (int Min, int Max)> { { Attributes.Armor, (7, 13) } } },
			{
				ItemTier.Uncommon,
				new Dictionary<Attributes, (int Min, int Max)>
					{ { Attributes.Armor, (13, 27) }, { Attributes.Health, (3, 7) } }
			},
			{
				ItemTier.Rare,
				new Dictionary<Attributes, (int Min, int Max)>
					{ { Attributes.Armor, (27, 33) }, { Attributes.Health, (7, 13) } }
			},
			{
				ItemTier.Epic,
				new Dictionary<Attributes, (int Min, int Max)>
				{
					{ Attributes.Armor, (33, 47) }, { Attributes.Health, (13, 27) },
					{ Attributes.RegenerationHealth, (3, 7) }
				}
			},
			{
				ItemTier.Legendary,
				new Dictionary<Attributes, (int Min, int Max)>
				{
					{ Attributes.Armor, (47, 53) }, { Attributes.Health, (27, 33) },
					{ Attributes.RegenerationHealth, (7, 13) }, { Attributes.Strength, (3, 7) }
				}
			},
			{
				ItemTier.Mythical,
				new Dictionary<Attributes, (int Min, int Max)>
				{
					{ Attributes.Armor, (53, 67) }, { Attributes.Health, (33, 67) },
					{ Attributes.RegenerationHealth, (13, 27) }, { Attributes.Strength, (7, 13) }
				}
			}
		};

		public static Dictionary<ItemTier, Dictionary<Attributes, (int Min, int Max)>> GlovesBuffs = new()
		{
			{ ItemTier.Common, new Dictionary<Attributes, (int Min, int Max)> { { Attributes.Armor, (3, 7) } } },
			{
				ItemTier.Uncommon,
				new Dictionary<Attributes, (int Min, int Max)>
					{ { Attributes.Armor, (7, 13) }, { Attributes.Strength, (1, 3) } }
			},
			{
				ItemTier.Rare,
				new Dictionary<Attributes, (int Min, int Max)>
				{
					{ Attributes.Armor, (13, 17) }, { Attributes.Strength, (3, 7) }, { Attributes.Dexterity, (1, 3) }
				}
			},
			{
				ItemTier.Epic,
				new Dictionary<Attributes, (int Min, int Max)>
				{
					{ Attributes.Armor, (17, 23) }, { Attributes.Strength, (7, 13) }, { Attributes.Dexterity, (3, 7) }
				}
			},
			{
				ItemTier.Legendary,
				new Dictionary<Attributes, (int Min, int Max)>
				{
					{ Attributes.Armor, (23, 37) }, { Attributes.Strength, (13, 17) },
					{ Attributes.Dexterity, (7, 13) }, { Attributes.Health, (3, 7) }
				}
			},
			{
				ItemTier.Mythical,
				new Dictionary<Attributes, (int Min, int Max)>
				{
					{ Attributes.Armor, (37, 63) }, { Attributes.Strength, (17, 33) },
					{ Attributes.Dexterity, (13, 27) }, { Attributes.Health, (7, 13) }
				}
			}
		};

		public static Dictionary<ItemTier, Dictionary<Attributes, (int Min, int Max)>> LegsBuffs = new()
		{
			{ ItemTier.Common, new Dictionary<Attributes, (int Min, int Max)> { { Attributes.Armor, (7, 13) } } },
			{
				ItemTier.Uncommon,
				new Dictionary<Attributes, (int Min, int Max)>
					{ { Attributes.Armor, (13, 27) }, { Attributes.Health, (3, 7) } }
			},
			{
				ItemTier.Rare,
				new Dictionary<Attributes, (int Min, int Max)>
					{ { Attributes.Armor, (27, 33) }, { Attributes.Health, (7, 13) } }
			},
			{
				ItemTier.Epic,
				new Dictionary<Attributes, (int Min, int Max)>
				{
					{ Attributes.Armor, (33, 47) }, { Attributes.Health, (13, 27) },
					{ Attributes.RegenerationHealth, (3, 7) }
				}
			},
			{
				ItemTier.Legendary,
				new Dictionary<Attributes, (int Min, int Max)>
				{
					{ Attributes.Armor, (47, 53) }, { Attributes.Health, (27, 33) },
					{ Attributes.RegenerationHealth, (7, 13) }, { Attributes.Dexterity, (3, 7) }
				}
			},
			{
				ItemTier.Mythical,
				new Dictionary<Attributes, (int Min, int Max)>
				{
					{ Attributes.Armor, (53, 67) }, { Attributes.Health, (33, 67) },
					{ Attributes.RegenerationHealth, (13, 27) }, { Attributes.Dexterity, (7, 13) }
				}
			}
		};

		public static Dictionary<ItemTier, Dictionary<Attributes, (int Min, int Max)>> BootsBuffs = new()
		{
			{
				ItemTier.Common,
				new Dictionary<Attributes, (int Min, int Max)>
					{ { Attributes.Armor, (3, 7) }, { Attributes.Agility, (1, 2) } }
			},
			{
				ItemTier.Uncommon,
				new Dictionary<Attributes, (int Min, int Max)>
					{ { Attributes.Armor, (7, 13) }, { Attributes.Agility, (1, 3) } }
			},
			{
				ItemTier.Rare,
				new Dictionary<Attributes, (int Min, int Max)>
					{ { Attributes.Armor, (13, 17) }, { Attributes.Agility, (3, 7) }, { Attributes.Dexterity, (1, 3) } }
			},
			{
				ItemTier.Epic,
				new Dictionary<Attributes, (int Min, int Max)>
				{
					{ Attributes.Armor, (17, 23) }, { Attributes.Agility, (7, 13) }, { Attributes.Dexterity, (3, 7) }
				}
			},
			{
				ItemTier.Legendary,
				new Dictionary<Attributes, (int Min, int Max)>
				{
					{ Attributes.Armor, (23, 37) }, { Attributes.Agility, (13, 17) }, { Attributes.Dexterity, (7, 13) },
					{ Attributes.Evasion, (1, 2) }
				}
			},
			{
				ItemTier.Mythical,
				new Dictionary<Attributes, (int Min, int Max)>
				{
					{ Attributes.Armor, (37, 63) }, { Attributes.Agility, (17, 33) },
					{ Attributes.Dexterity, (13, 27) }, { Attributes.Evasion, (3, 7) }
				}
			}
		};

		public static Dictionary<ItemTier, Dictionary<Attributes, (int Min, int Max)>> RingBuffs = new()
		{
			{ ItemTier.Common, new Dictionary<Attributes, (int Min, int Max)> { { Attributes.Agility, (1, 2) } } },
			{
				ItemTier.Uncommon,
				new Dictionary<Attributes, (int Min, int Max)>
					{ { Attributes.Agility, (1, 3) }, { Attributes.Luck, (1, 2) } }
			},
			{
				ItemTier.Rare,
				new Dictionary<Attributes, (int Min, int Max)>
					{ { Attributes.Agility, (3, 7) }, { Attributes.Luck, (1, 3) } }
			},
			{
				ItemTier.Epic,
				new Dictionary<Attributes, (int Min, int Max)>
				{
					{ Attributes.Agility, (7, 13) }, { Attributes.Luck, (3, 7) },
					{ Attributes.CriticalHitChance, (1, 2) }
				}
			},
			{
				ItemTier.Legendary,
				new Dictionary<Attributes, (int Min, int Max)>
				{
					{ Attributes.Agility, (13, 17) }, { Attributes.Luck, (7, 13) },
					{ Attributes.CriticalHitChance, (3, 7) }
				}
			},
			{
				ItemTier.Mythical,
				new Dictionary<Attributes, (int Min, int Max)>
				{
					{ Attributes.Agility, (17, 33) }, { Attributes.Luck, (13, 27) },
					{ Attributes.CriticalHitChance, (7, 13) }
				}
			}
		};

		public static Dictionary<ItemTier, Dictionary<Attributes, (int Min, int Max)>> EarringBuffs = new()
		{
			{ ItemTier.Common, new Dictionary<Attributes, (int Min, int Max)> { { Attributes.Intellect, (1, 2) } } },
			{
				ItemTier.Uncommon,
				new Dictionary<Attributes, (int Min, int Max)>
					{ { Attributes.Intellect, (1, 3) }, { Attributes.Luck, (1, 2) } }
			},
			{
				ItemTier.Rare,
				new Dictionary<Attributes, (int Min, int Max)>
					{ { Attributes.Intellect, (3, 7) }, { Attributes.Luck, (1, 3) } }
			},
			{
				ItemTier.Epic,
				new Dictionary<Attributes, (int Min, int Max)>
					{ { Attributes.Intellect, (7, 13) }, { Attributes.Luck, (3, 7) }, { Attributes.Mana, (1, 3) } }
			},
			{
				ItemTier.Legendary,
				new Dictionary<Attributes, (int Min, int Max)>
					{ { Attributes.Intellect, (13, 17) }, { Attributes.Luck, (7, 13) }, { Attributes.Mana, (3, 7) } }
			},
			{
				ItemTier.Mythical,
				new Dictionary<Attributes, (int Min, int Max)>
					{ { Attributes.Intellect, (17, 33) }, { Attributes.Luck, (13, 27) }, { Attributes.Mana, (7, 13) } }
			}
		};

		public static Dictionary<ItemTier, Dictionary<Attributes, (int Min, int Max)>> NecklaceBuffs = new()
		{
			{ ItemTier.Common, new Dictionary<Attributes, (int Min, int Max)> { { Attributes.Stamina, (1, 2) } } },
			{
				ItemTier.Uncommon,
				new Dictionary<Attributes, (int Min, int Max)>
					{ { Attributes.Stamina, (1, 3) }, { Attributes.Luck, (1, 2) } }
			},
			{
				ItemTier.Rare,
				new Dictionary<Attributes, (int Min, int Max)>
					{ { Attributes.Stamina, (3, 7) }, { Attributes.Luck, (1, 3) } }
			},
			{
				ItemTier.Epic,
				new Dictionary<Attributes, (int Min, int Max)>
					{ { Attributes.Stamina, (7, 13) }, { Attributes.Luck, (3, 7) }, { Attributes.Health, (1, 3) } }
			},
			{
				ItemTier.Legendary,
				new Dictionary<Attributes, (int Min, int Max)>
					{ { Attributes.Stamina, (13, 17) }, { Attributes.Luck, (7, 13) }, { Attributes.Health, (3, 7) } }
			},
			{
				ItemTier.Mythical,
				new Dictionary<Attributes, (int Min, int Max)>
					{ { Attributes.Stamina, (17, 33) }, { Attributes.Luck, (13, 27) }, { Attributes.Health, (7, 13) } }
			}
		};
	}
}