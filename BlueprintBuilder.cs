using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace TheBluePrinter
{
    /// <summary>
    /// Warning! this code and almost all of the following code is very filthy
    /// </summary>
    public class BlueprintBuilder
    {
        




        /// <summary>
        /// Takes the entities from BuildImageAssembler() and converts them to JSON text with all connections applied
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static string BuildBlueprint(List<object> objects, int width = 0, int BeltTier = 2)
        {
            string beltType = "express-transport-belt";
            string underGroundType = "express-underground-belt";
            string splitterType = "express-splitter";
            if (BeltTier == 0)
            {
                beltType = "transport-belt";
                underGroundType = "underground-belt";
                splitterType = "splitter";
            }
            else if (BeltTier == 1)
            {
                beltType = "fast-transport-belt";
                underGroundType = "fast-underground-belt";
                splitterType = "fast-splitter";
            }
            else if (BeltTier == 2)
            {
                beltType = "express-transport-belt";
                underGroundType = "express-underground-belt";
                splitterType = "express-splitter";
            }

            List<Blueprint.entity> allEntities = new List<Blueprint.entity>();
            Dictionary<object, Blueprint.entity> connectionLookup = new Dictionary<object, Blueprint.entity>();

            Dictionary<Substation, Blueprint.entity> neighborLookup = new Dictionary<Substation, Blueprint.entity>();
            foreach (object o in objects)
            {
                if (o.GetType() == typeof(RequesterChest))
                {
                    RequesterChest chest = (RequesterChest)o;
                    allEntities.Add(new Blueprint.entity("logistic-chest-requester", new Blueprint.entityComponent[] { chest.position.AsBlueprintPosition, new Blueprint.request_filters(chest.request, 12) }));

                }
                if (o.GetType() == typeof(Belt))
                {
                    Belt belt = (Belt)o;
                    Blueprint.entity b = new Blueprint.entity(beltType, new Blueprint.entityComponent[] { belt.position.AsBlueprintPosition, new Blueprint.direction(belt.rotation) });
                    allEntities.Add(b);
                    connectionLookup.Add(belt, b);
                }
                if (o.GetType() == typeof(Inserter))
                {
                    Inserter inserter = (Inserter)o;
                    Blueprint.entity inserterEntity = new Blueprint.entity("inserter", new Blueprint.entityComponent[] { inserter.position.AsBlueprintPosition, new Blueprint.direction(inserter.rotation), new Blueprint.insertercontrolbehavior() });
                    connectionLookup.Add(inserter, inserterEntity);
                    allEntities.Add(inserterEntity);
                }
                if (o.GetType() == typeof(StackInserter))
                {
                    StackInserter inserter = (StackInserter)o;
                    Blueprint.entity inserterEntity = new Blueprint.entity("stack-inserter", new Blueprint.entityComponent[] { inserter.position.AsBlueprintPosition, new Blueprint.direction(inserter.rotation), new Blueprint.insertercontrolbehavior() });
                    //connectionLookup.Add(inserter, inserterEntity);
                    allEntities.Add(inserterEntity);
                }
                if (o.GetType() == typeof(Substation))
                {
                    Substation s = (Substation)o;
                    Blueprint.entity SubstationEntity = new Blueprint.entity("substation", new Blueprint.entityComponent[] { s.position.AsBlueprintPosition });
                    allEntities.Add(SubstationEntity);
                    neighborLookup.Add(s, SubstationEntity);
                    connectionLookup.Add(s, SubstationEntity);
                }
                if (o.GetType() == typeof(roboport))
                {
                    roboport r = (roboport)o;
                    allEntities.Add(new Blueprint.entity("roboport", new Blueprint.entityComponent[] { r.position.AsBlueprintPosition }));
                }
                if (o.GetType() == typeof(InfinityChest))
                {
                    InfinityChest I = (InfinityChest)o;
                    allEntities.Add(new Blueprint.entity("infinity-chest", new Blueprint.entityComponent[] { I.position.AsBlueprintPosition, new Blueprint.infinity_settings(I.item, I.count) }));
                }
                if (o.GetType() == typeof(UnderGround))
                {
                    UnderGround U = (UnderGround)o;
                    allEntities.Add(new Blueprint.entity(underGroundType, new Blueprint.entityComponent[] { U.position.AsBlueprintPosition, new Blueprint.direction(U.rotation), new Blueprint.type(U.TFinputOrOutput) }));
                }
                if (o.GetType() == typeof(Splitter))
                {
                    Splitter S = (Splitter)o;
                    allEntities.Add(new Blueprint.entity(splitterType, new Blueprint.entityComponent[] { S.position.AsBlueprintPosition, new Blueprint.direction(S.rotation), new Blueprint.input_priority(S.input_priority ? "left" : "right") }));

                }
                if (o.GetType() == typeof(OutputPrioritySplitter))
                {
                    OutputPrioritySplitter w = (OutputPrioritySplitter)o;
                    allEntities.Add(new Blueprint.entity(splitterType, new Blueprint.entityComponent[] { w.position.AsBlueprintPosition, new Blueprint.direction(w.rotation), new Blueprint.output_priority(w.output_priority ? "left" : "right") }));

                }
                if (o.GetType() == typeof(PassiveProviderChest))
                {
                    PassiveProviderChest P = (PassiveProviderChest)o;
                    allEntities.Add(new Blueprint.entity("logistic-chest-passive-provider", new Blueprint.entityComponent[] { P.position.AsBlueprintPosition }));
                }
            }

            foreach (object o2 in connectionLookup.Keys)
            {
                if (o2.GetType() == typeof(Inserter))
                {
                    Inserter i = (Inserter)o2;
                    if (i.connections.Count > 0)
                        connectionLookup[i].components.Add(new Blueprint.connections(new int[] { connectionLookup[i.connections[0]].GetEntityNumber() }));
                }
                if (o2.GetType() == typeof(Belt))
                {
                    Belt b = (Belt)o2;
                    if (b.connections.Count > 0)
                        connectionLookup[b].components.Add(new Blueprint.connections(new int[] { connectionLookup[b.connections[0]].GetEntityNumber() }));
                    if (b.hasSignal)
                        connectionLookup[b].components.Add(new Blueprint.beltControlBehavior());
                }
            }

            foreach (Substation s in neighborLookup.Keys)
            {
                if (s.neighbors.Count > 0)
                {
                    int[] neighbors = new int[s.neighbors.Count];
                    for (int i = 0; i < s.neighbors.Count; i++)
                    {
                        neighbors[i] = neighborLookup[s.neighbors[i]].GetEntityNumber();
                    }
                    neighborLookup[s].components.Add(new Blueprint.neighbors(neighbors));
                }
                if (s.connections.Count > 0)
                {
                    int[] connections = new int[s.connections.Count];
                    for (int i = 0; i < s.connections.Count; i++)
                    {
                        connections[i] = connectionLookup[s.connections[i]].GetEntityNumber();
                    }
                    connectionLookup[s].components.Add(new Blueprint.connections(connections));
                }
            }

            string result = "{\"blueprint\": { \"icons\": [{\"signal\": { \"type\": \"item\",\"name\": \"express-transport-belt\"},\"index\": 1},{\"signal\": { \"type\": \"item\",\"name\": \"logistic-chest-requester\"},\"index\": 2}], \"entities\": [";
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(result);
            if (width > 0)
            {
                stringBuilder.Append(GetDriverString(width));
                

            }
            for (int i = 0; i < allEntities.Count; i++)
            {
                Blueprint.entity e = allEntities[i];
                //result += e.getJson();
                stringBuilder.Append(e.getJson());
                if (i < allEntities.Count - 1)
                {
                    // result += ",";
                    stringBuilder.Append(',');
                }
            }
            //result += "],\"item\": \"blueprint\",\"version\": 281479274168320}}";
            stringBuilder.Append("],\"item\": \"blueprint\",\"version\": 281479274168320}}");
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Main function to determin positions of all entities in the blueprint and how they connect to each other
        /// </summary>
        /// <param name="idMap">Takes a 2d array containing item ids which map to their index in the allItemsList</param>
        /// <returns></returns>
        public static List<object> BuildImageAssembler(int[,] idMap)
        {
           
            List<object> entities = new List<object>();                         //Final List of entities
            bool top = true;                                                    //whether the current row will be placed on top, with the inserters facing down
            bool placeRoboPorts = true;                                         //place roboports every other row, i probably could have used top for this

            // stores all substations in the last row so the substations in the next row can connect to them
            int substationRows = (int)Math.Ceiling((double)idMap.GetLength(1) / 18.0) + 1;
            Substation[] lastRow = new Substation[substationRows];

            // stores the last placed substation in a row so the next substation can connect to it
            Substation lastSubstation = null;

            // stores the last header substation at the front of every two rows to connect the first inserters to the circut network
            Substation lastHeaderSubstation = null;
            Inserter[] lastHeaderBufferInserters = new Inserter[2];

            //for a last minute changes im going to use offset variables so I can push parts of the assembler out
            float AssemblerOffset = 1f;
            float MergeOffset = 3f;
            for (int y = 0; y < idMap.GetLength(0); y++)
            {
                if (y % 2 == 0)
                {
                    //Adds the first substation to each row in a fixed position
                    Substation substation = new Substation(new pos(-3f + AssemblerOffset, y * 5 - 2));

                    if (lastRow[0] != null)
                    {
                        substation.neighbors.Add(lastRow[0]);
                    }
                    lastRow[0] = substation;
                    lastSubstation = substation;
                    entities.Add(substation);
                    placeRoboPorts = true;

                    //Adds the cap to the front of every 2 rows combining them to 1 belt
                    //Also adds image buffer to the front

                    

                    entities.AddRange(new object[]
                    {
                        new Belt(new pos(-3.5f + AssemblerOffset, y * 5 + 2.5f), 4),
                        new Belt(new pos(-2.5f + AssemblerOffset, y * 5 + 2.5f), 6),
                        new Belt(new pos(-1.5f + AssemblerOffset, y * 5 + 2.5f), 6),
                        new Belt(new pos(-0.5f + AssemblerOffset, y * 5 + 2.5f), 6),
                        new Belt(new pos(-4.5f + AssemblerOffset, y * 5 + 3.5f), 6),
                        new Belt(new pos(-3.5f + AssemblerOffset, y * 5 + 3.5f), 6),
                        new Belt(new pos(-2.5f + AssemblerOffset, y * 5 + 3.5f), 4),
                        new Belt(new pos(-1.5f + AssemblerOffset, y * 5 + 3.5f), 6),
                        new Belt(new pos(-0.5f + AssemblerOffset, y * 5 + 3.5f), 6),
                        new Belt(new pos(-3.5f + AssemblerOffset, y * 5 + 4.5f), 0),
                        new Belt(new pos(-2.5f + AssemblerOffset, y * 5 + 4.5f), 6),
                        new UnderGround(new pos(-4.5f + AssemblerOffset, y * 5 + 1.5f), 0, false),
                        new UnderGround(new pos(-4.5f + AssemblerOffset, y * 5 + 5.5f), 0, true)
                        
                    });
                    if (y < idMap.GetLength(0) - 2)
                    {
                        entities.AddRange(new object[]
                        {
                            new Belt(new pos(-4.5f + AssemblerOffset, y * 5 + 6.5f), 0),
                            new Belt(new pos(-4.5f + AssemblerOffset, y * 5 + 7.5f), 0),
                            new Belt(new pos(-4.5f + AssemblerOffset, y * 5 + 8.5f), 0),
                            new Belt(new pos(-4.5f + AssemblerOffset, y * 5 + 9.5f), 0),
                            new Belt(new pos(-4.5f + AssemblerOffset, y * 5 + 10.5f), 0)
                        });
                    }

                        Inserter bufferTop = new Inserter(new pos(-4.5f + AssemblerOffset, y * 5 + 2.5f), 0);
                    Inserter bufferBottom = new Inserter(new pos(-4.5f + AssemblerOffset, y * 5 + 4.5f), 4);

                    substation.connections.Add(bufferTop);
                    substation.connections.Add(bufferBottom);

                    entities.Add(bufferTop);
                    entities.Add(bufferBottom);

                    if (lastHeaderSubstation != null)
                    {
                        substation.connections.Add(lastHeaderSubstation);
                    }
                    lastHeaderSubstation = substation;
                    lastHeaderBufferInserters[0] = bufferTop;
                    lastHeaderBufferInserters[1] = bufferBottom;

                    
                }
                else
                {
                    placeRoboPorts = false;
                }

                Inserter lastInserter = null;       // stores the last placed inserter so the next inserter can connect to it with wires

                //Main loops for rows of requestor chests
                for (int x = 0; x < idMap.GetLength(1); x++)
                {
                    if (x % 18 == 0 && placeRoboPorts)      //every 18 positions, place a substation and a roboport to the left of it
                    {

                        Substation station = new Substation(new pos(x + 9f + AssemblerOffset, y * 5 - 2));
                        if (lastRow[(x / 18) + 1] != null)
                        {
                            station.neighbors.Add(lastRow[(x / 18) + 1]);
                        }
                        lastRow[(x / 18) + 1] = station;
                        station.neighbors.Add(lastSubstation);
                        lastSubstation = station;

                        entities.Add(station);
                        entities.Add(new roboport(new pos(x + AssemblerOffset, y * 5 - 2)));
                    }
                    if (top)
                    {
                        entities.Add(new RequesterChest(new pos(x + AssemblerOffset + 0.5f, y * 5 + 0.5f), Item.AllItems[idMap[y, x]].Name));
                        Inserter inserter = new Inserter(new pos(x + AssemblerOffset + 0.5f, y * 5 + 1.5f), 0);
                        if (lastInserter != null)
                        {
                            inserter.connections.Add(lastInserter);
                        }
                        if (x == 0)
                        {
                            lastHeaderBufferInserters[0].connections.Add(inserter);
                        }
                        lastInserter = inserter;
                        entities.Add(inserter);
                        entities.Add(new Belt(new pos(x + AssemblerOffset + 0.5f, y * 5 + 2.5f), 6));

                    }
                    else
                    {
                        entities.Add(new RequesterChest(new pos(x + AssemblerOffset + 0.5f, y * 5 + 0.5f), Item.AllItems[idMap[y, x]].Name));
                        Inserter inserter = new Inserter(new pos(x + AssemblerOffset + 0.5f, y * 5 - 0.5f), 4);
                        if (lastInserter != null)
                        {
                            inserter.connections.Add(lastInserter);
                        }
                        if (x == 0)
                        {
                            lastHeaderBufferInserters[1].connections.Add(inserter);
                        }
                        lastInserter = inserter;
                        entities.Add(inserter);
                        entities.Add(new Belt(new pos(x + AssemblerOffset + 0.5f, y * 5 - 1.5f), 6));
                    }
                }
                lastSubstation = null;
                lastInserter = null;
                top = !top;
            }


            //build assembling belts
            int belts = (int)Math.Ceiling(idMap.GetLength(0) / 2f);
            for (int y = 0; y < belts; y++)
            {
                int ry = belts - y;
                for (int i = 0; i < ry - 1; i++)
                {
                    entities.Add(new Belt(new pos(-4.5f - i, 3.5f + y * 10f), 6));
                }
                for (int u = 0; u < (ry - 1) * 10 + 3; u++)
                {
                    entities.Add(new Belt(new pos(-4.5f - ry + 1, 3.5f + y * 10 + u), 4));
                }
            }

            //build gate and cache
            float cacheY = 3.5f + belts * 10 - 7;
            int cacheLength = (int)Math.Ceiling(idMap.GetLength(1) * 2 / 4f);
            Belt[] gates = new Belt[belts];
            for (int q = 0; q < belts; q++)
            {
                for (int v = 0; v < cacheLength; v++)
                {
                    entities.Add(new Belt(new pos(-4.5f - q, cacheY + v), 4));
                }
                gates[q] = new Belt(new pos(-4.5f - q, cacheY + cacheLength), 4);
                gates[q].hasSignal = true;
                if (q > 0)
                {
                    gates[q].connections.Add(gates[q - 1]);
                }
            }
            entities.AddRange(gates);

            //Build front buffer passthrough
            int undergroundBeltPlacementCounter = 0;
            float FBy = cacheY + cacheLength + 1f;
            float FBx = -4.5f;
            entities.Add(new UnderGround(new pos(FBx + 1f, FBy + 1f), 2, false));
            entities.Add(new Belt(new pos(FBx + 2f, FBy + 1f), 0));
            entities.Add(new Belt(new pos(FBx + 2f, FBy), 6));
            entities.Add(new Belt(new pos(FBx + 1f, FBy), 0));
            entities.Add(new Belt(new pos(FBx + 1f, FBy - 1f), 0));

            for (int k = 0; k < cacheLength; k++)
            {
                entities.Add(new Belt(new pos(FBx + 1f, FBy - 2f - k), 0));

            }

            for (int h = 0; h < belts; h++)
            {
                if (undergroundBeltPlacementCounter == 8)   //every 8 belts place an underground
                {
                    entities.AddRange(new object[]
                    {
                       new UnderGround(new pos(FBx - h, FBy), 4, true),
                       new UnderGround(new pos(FBx - h, FBy + 1f), 2, true),
                       new UnderGround(new pos(FBx - h, FBy + 2f), 4, false)

                    });
                    undergroundBeltPlacementCounter++;
                }
                else if (undergroundBeltPlacementCounter == 9)
                {
                    undergroundBeltPlacementCounter = 0;
                    entities.AddRange(new object[]
                    {
                       new UnderGround(new pos(FBx - h, FBy), 4, true),
                       new UnderGround(new pos(FBx - h, FBy + 1f), 2, false),
                       new UnderGround(new pos(FBx - h, FBy + 2f), 4, false)

                    });
                }
                else
                {
                    entities.AddRange(new object[]
                    {
                        
                        new Belt(new pos(FBx - h, FBy),4),
                        new Belt(new pos(FBx - h, FBy + 1f),4),
                        new Belt(new pos(FBx - h, FBy + 2f),4),
                    });
                    undergroundBeltPlacementCounter++;
                }
            }

            if (undergroundBeltPlacementCounter == 9)
            {
                entities.Add(new Belt(new pos(FBx - belts, FBy + 1f), 2));
            }
            else
            {
                entities.Add(new UnderGround(new pos(FBx - belts, FBy + 1f), 2, true));
            }

            //add spliter and padding before the merge to feed the buffer
            entities.Add(new Belt(new pos(FBx - belts - 1f, FBy + 1f), 2));
            entities.Add(new OutputPrioritySplitter(new pos(FBx - belts - 1.5f, FBy), 4, false));
            entities.Add(new Belt(new pos(FBx - belts - 2f, FBy + 1f), 4));
            entities.Add(new Belt(new pos(FBx - belts - 2f, FBy + 2f), 4));

            for (int c = 1; c < belts; c++) //add padding before the merge starting at 1 to skip the first one we already did
            {
                entities.Add(new Belt(new pos(FBx - belts - 2f - c, FBy     ), 4));
                entities.Add(new Belt(new pos(FBx - belts - 2f - c, FBy + 1f), 4));
                entities.Add(new Belt(new pos(FBx - belts - 2f - c, FBy + 2f), 4));
            }


            //build merge
            float mergeX = -3.5f - belts;
            float mergeY = cacheY + cacheLength + 1f + MergeOffset;
            int groups = (int)Math.Floor(belts / 4f);
            Log.New("groups:" + groups.ToString());

            for (int h = 0; h < belts; h++)
            {
                //each group of 4 belts
                int f = (int)Math.Floor(h / 4f);

                int line = h - (f * 4);

                Log.New("h:" + h.ToString() + "  f:" + f.ToString() + "  l:" + line.ToString());
                //merge assembly constants
                float MAssemblyX = mergeX - 2f - belts + (4f * f);
                float MAssemblyY = mergeY + (10f * f);


                //build each line specific to its index, very lazy way to do this but its easy and works with the garbage mess I already made
                if (line == 0)
                {
                    //add one space to the front for no reason
                    entities.Add(new Belt(new pos(mergeX + (f * 4), mergeY), 4));

                    //add 10 belts
                    for (int l = 0; l < 10 * f; l++)
                    {
                        entities.Add(new Belt(new pos(mergeX + (f * 4), mergeY + 1 + l), 4));

                    }
                    //build 4x turn to the right
                    entities.Add(new Belt(new pos(mergeX + (f * 4), mergeY + 1 + (10 * f)), 6));

                    //build merge assembly
                    entities.Add(new Belt(new pos(MAssemblyX, MAssemblyY), 4));
                    entities.Add(new Belt(new pos(MAssemblyX, MAssemblyY + 1f), 4));
                    entities.Add(new Splitter(new pos(MAssemblyX + 0.5f, MAssemblyY + 2f), 4, true));
                    entities.Add(new Belt(new pos(MAssemblyX, MAssemblyY + 3f), 4));
                    entities.Add(new Belt(new pos(MAssemblyX, MAssemblyY + 4f), 4));
                    entities.Add(new Belt(new pos(MAssemblyX, MAssemblyY + 5f), 4));
                    entities.Add(new Belt(new pos(MAssemblyX, MAssemblyY + 6f), 4));
                    entities.Add(new Belt(new pos(MAssemblyX, MAssemblyY + 7f), 4));
                    entities.Add(new Belt(new pos(MAssemblyX, MAssemblyY + 8f), 4));
                    entities.Add(new Belt(new pos(MAssemblyX, MAssemblyY + 9f), 4));
                    entities.Add(new Belt(new pos(MAssemblyX + 1f, MAssemblyY + 1f), 4));
                    entities.Add(new Belt(new pos(MAssemblyX + 2f, MAssemblyY + 1f), 6));
                    entities.Add(new Belt(new pos(MAssemblyX + 3f, MAssemblyY + 1f), 6));
                    entities.Add(new Belt(new pos(MAssemblyX + 4f, MAssemblyY + 1f), 6));
                    

                    //connect merge and 4x turn
                    for (int v = 0; v < belts - 3; v++)
                    {
                        entities.Add(new Belt(new pos(MAssemblyX + 5f + v, MAssemblyY + 1f), 6));
                    }

                    //connect underground belts to the beginning
                    for (int z = 0; z < f; z++)
                    {
                        float offset = 10f * z;
                        entities.Add(new UnderGround(new pos(MAssemblyX, MAssemblyY - 10f - offset), 4, true));
                        entities.Add(new UnderGround(new pos(MAssemblyX, MAssemblyY - 1f - offset), 4, false));
                    }

                    //connect regular belts to the end
                    for (int r = f; r < groups; r++)
                    {
                        for (int o = 0; o < 10; o++)
                        {
                            entities.Add(new Belt(new pos(MAssemblyX, MAssemblyY + (10 * (groups - r)) + o), 4));
                        }
                    }


                }
                else if (line == 1)
                {
                    //add one space to the front for no reason
                    entities.Add(new Belt(new pos(mergeX + 1 + (f * 4), mergeY), 4));

                    //add 10 belts
                    for (int l = 0; l < 10 * f; l++)
                    {

                        entities.Add(new Belt(new pos(mergeX + 1 + (f * 4), mergeY + 1 + l), 4));

                    }
                    //build 4x turn to the right
                    entities.Add(new Belt(new pos(mergeX + 1 + (f * 4), mergeY + 1 + (10 * f)), 4));
                    entities.Add(new Belt(new pos(mergeX + 1 + (f * 4), mergeY + 2 + (10 * f)), 6));
                    entities.Add(new Belt(new pos(mergeX + (f * 4), mergeY + 2 + (10 * f)), 6));

                    //build merge assembly
                    entities.Add(new UnderGround(new pos(MAssemblyX + 1f, MAssemblyY), 4, true));
                    entities.Add(new UnderGround(new pos(MAssemblyX + 1f, MAssemblyY + 3f), 4, false));
                    entities.Add(new Splitter(new pos(MAssemblyX + 1.5f, MAssemblyY + 4f), 4, true));
                    entities.Add(new Belt(new pos(MAssemblyX + 1f, MAssemblyY + 5f), 4));
                    entities.Add(new Belt(new pos(MAssemblyX + 1f, MAssemblyY + 6f), 4));
                    entities.Add(new Belt(new pos(MAssemblyX + 1f, MAssemblyY + 7f), 4));
                    entities.Add(new Belt(new pos(MAssemblyX + 1f, MAssemblyY + 8f), 4));
                    entities.Add(new Belt(new pos(MAssemblyX + 1f, MAssemblyY + 9f), 4));
                    entities.Add(new Belt(new pos(MAssemblyX + 2f, MAssemblyY + 2f), 4));
                    entities.Add(new Belt(new pos(MAssemblyX + 3f, MAssemblyY + 2f), 6));
                    entities.Add(new Belt(new pos(MAssemblyX + 4f, MAssemblyY + 2f), 6));
                    entities.Add(new Belt(new pos(MAssemblyX + 2f, MAssemblyY + 3f), 4));

                    //connect merge and 4x turn
                    for (int v = 0; v < belts - 3; v++)
                    {
                        entities.Add(new Belt(new pos(MAssemblyX + 5f + v, MAssemblyY + 2f), 6));
                    }

                    //connect underground belts to the beginning
                    for (int z = 0; z < f; z++)
                    {
                        float offset = 10f * z;
                        entities.Add(new UnderGround(new pos(MAssemblyX + 1f, MAssemblyY - 10f - offset), 4, true));
                        entities.Add(new UnderGround(new pos(MAssemblyX + 1f, MAssemblyY - 1f - offset), 4, false));
                    }

                    //connect regular belts to the end
                    for (int r = f; r < groups; r++)
                    {
                        for (int o = 0; o < 10; o++)
                        {
                            entities.Add(new Belt(new pos(MAssemblyX + 1f, MAssemblyY + (10 * (groups - r)) + o), 4));
                        }
                    }

                }
                else if (line == 2)
                {
                    //add one space to the front for no reason
                    entities.Add(new Belt(new pos(mergeX + 2 + (f * 4), mergeY), 4));

                    //add 10 belts
                    for (int l = 0; l < 10 * f; l++)
                    {

                        entities.Add(new Belt(new pos(mergeX + 2 + (f * 4), mergeY + 1 + l), 4));

                    }
                    //build 4x turn to the right
                    entities.Add(new Belt(new pos(mergeX + 2 + (f * 4), mergeY + 1 + (10 * f)), 4));
                    entities.Add(new Belt(new pos(mergeX + 2 + (f * 4), mergeY + 2 + (10 * f)), 4));
                    entities.Add(new Belt(new pos(mergeX + 2 + (f * 4), mergeY + 3 + (10 * f)), 6));
                    entities.Add(new Belt(new pos(mergeX + 1 + (f * 4), mergeY + 3 + (10 * f)), 6));
                    entities.Add(new Belt(new pos(mergeX + (f * 4), mergeY + 3 + (10 * f)), 6));

                    //build merge assembly
                    entities.Add(new UnderGround(new pos(MAssemblyX + 2f, MAssemblyY), 4, true));
                    entities.Add(new UnderGround(new pos(MAssemblyX + 2f, MAssemblyY + 5f), 4, false));
                    entities.Add(new Splitter(new pos(MAssemblyX + 2.5f, MAssemblyY + 6f), 4, true));
                    entities.Add(new Belt(new pos(MAssemblyX + 2f, MAssemblyY + 7f), 4));
                    entities.Add(new Belt(new pos(MAssemblyX + 2f, MAssemblyY + 8f), 4));
                    entities.Add(new Belt(new pos(MAssemblyX + 2f, MAssemblyY + 9f), 4));
                    entities.Add(new Belt(new pos(MAssemblyX + 3f, MAssemblyY + 3f), 4));
                    entities.Add(new Belt(new pos(MAssemblyX + 3f, MAssemblyY + 4f), 4));
                    entities.Add(new Belt(new pos(MAssemblyX + 3f, MAssemblyY + 5f), 4));
                    entities.Add(new Belt(new pos(MAssemblyX + 4f, MAssemblyY + 3f), 6));

                    //connect merge and 4x turn
                    for (int v = 0; v < belts - 3; v++)
                    {
                        entities.Add(new Belt(new pos(MAssemblyX + 5f + v, MAssemblyY + 3f), 6));
                    }

                    //connect underground belts to the beginning
                    for (int z = 0; z < f; z++)
                    {
                        float offset = 10f * z;
                        entities.Add(new UnderGround(new pos(MAssemblyX + 2f, MAssemblyY - 10f - offset), 4, true));
                        entities.Add(new UnderGround(new pos(MAssemblyX + 2f, MAssemblyY - 1f - offset), 4, false));
                    }

                    //connect regular belts to the end
                    for (int r = f; r < groups; r++)
                    {
                        for (int o = 0; o < 10; o++)
                        {
                            entities.Add(new Belt(new pos(MAssemblyX + 2f, MAssemblyY + (10 * (groups - r)) + o), 4));
                        }
                    }

                }
                else if (line == 3)
                {
                    //add one space to the front for no reason
                    entities.Add(new Belt(new pos(mergeX + 3 + (f * 4), mergeY), 4));

                    //add 10 belts
                    for (int l = 0; l < 10 * f; l++)
                    {
                        entities.Add(new Belt(new pos(mergeX + 3 + (f * 4), mergeY + 1 + l), 4));
                    }
                    //build 4x turn to the right
                    entities.Add(new Belt(new pos(mergeX + 3 + (f * 4), mergeY + 1 + (10 * f)), 4));
                    entities.Add(new Belt(new pos(mergeX + 3 + (f * 4), mergeY + 2 + (10 * f)), 4));
                    entities.Add(new Belt(new pos(mergeX + 3 + (f * 4), mergeY + 3 + (10 * f)), 4));
                    entities.Add(new Belt(new pos(mergeX + 3 + (f * 4), mergeY + 4 + (10 * f)), 6));
                    entities.Add(new Belt(new pos(mergeX + 2 + (f * 4), mergeY + 4 + (10 * f)), 6));
                    entities.Add(new Belt(new pos(mergeX + 1 + (f * 4), mergeY + 4 + (10 * f)), 6));
                    entities.Add(new Belt(new pos(mergeX + (f * 4), mergeY + 4 + (10 * f)), 6));

                    //build merge assembly
                    entities.Add(new UnderGround(new pos(MAssemblyX + 3f, MAssemblyY), 4, true));
                    entities.Add(new UnderGround(new pos(MAssemblyX + 3f, MAssemblyY + 7f), 4, false));
                    entities.Add(new Splitter(new pos(MAssemblyX + 3.5f, MAssemblyY + 8f), 4, true));
                    entities.Add(new Belt(new pos(MAssemblyX + 3f, MAssemblyY + 9f), 4));
                    entities.Add(new Belt(new pos(MAssemblyX + 4f, MAssemblyY + 4f), 4));
                    entities.Add(new Belt(new pos(MAssemblyX + 4f, MAssemblyY + 5f), 4));
                    entities.Add(new Belt(new pos(MAssemblyX + 4f, MAssemblyY + 6f), 4));
                    entities.Add(new Belt(new pos(MAssemblyX + 4f, MAssemblyY + 7f), 4));

                    //connect merge and 4x turn
                    for (int v = 0; v < belts - 3; v++)
                    {
                        entities.Add(new Belt(new pos(MAssemblyX + 5f + v, MAssemblyY + 4f), 6));
                    }

                    //connect underground belts to the beginning
                    for (int z = 0; z < f; z++)
                    {
                        float offset = 10f * z;
                        entities.Add(new UnderGround(new pos(MAssemblyX + 3f, MAssemblyY - 10f - offset), 4, true));
                        entities.Add(new UnderGround(new pos(MAssemblyX + 3f, MAssemblyY - 1f - offset), 4, false));
                    }

                    //connect regular belts to the end
                    for (int r = f; r < groups; r++)
                    {
                        for (int o = 0; o < 10; o++)
                        {
                            entities.Add(new Belt(new pos(MAssemblyX + 3f, MAssemblyY + (10 * (groups - r)) + o), 4));
                        }
                    }

                }




            }

            return entities;
        }

        public static List<object> BuildAllInfinityChests()
        {
            List<object> result = new List<object>();

            int k = 0;
            foreach (Item I in Item.AllItems)
            {
                bool isHidden = false;
                foreach (string s in I.Flags)
                {
                    if (s == "hidden") isHidden = true;
                }

                if (!isHidden) { 
                    result.Add(new InfinityChest(new pos(k + 0.5f, 0f), I.Name, 100));
                    result.Add(new StackInserter(new pos(k + 0.5f, 1f), 0));
                    result.Add(new PassiveProviderChest(new pos(k + 0.5f, 2f)));
                    k++;
                }
            }

            return result;
        }

        public struct pos
        {
            float x;
            float y;
            public pos(float x, float y)
            {
                this.x = x;
                this.y = y;
            }

            public Blueprint.position AsBlueprintPosition => new Blueprint.position(x, y);
        }

        public class RequesterChest
        {
            public pos position;
            public string request;

            public RequesterChest(pos position, string request)
            {
                this.position = position;
                this.request = request;
            }
        }

        public class Inserter
        {
            public pos position;
            public int rotation;    //up = 0, right = 2, down = 4, left = 6  grab from side
            public List<Inserter> connections = new List<Inserter>();

            public Inserter(pos position, int rotation)
            {
                this.position = position;
                this.rotation = rotation;
            }
        }
        
        public class StackInserter
        {
            public pos position;
            public int rotation;    //up = 0, right = 2, down = 4, left = 6  grab from side
            public List<Inserter> connections = new List<Inserter>();

            public StackInserter(pos position, int rotation)
            {
                this.position = position;
                this.rotation = rotation;
            }
        }

        public class Splitter
        {
            public pos position;
            public int rotation;
            public bool input_priority;

            public Splitter(pos position, int rotation)
            {
                this.position = position;
                this.rotation = rotation;
            }
            public Splitter(pos position, int rotation, bool TFLeftOrRight)
            {
                this.position = position;
                this.rotation = rotation;
                this.input_priority = TFLeftOrRight;
            }

        }

        public class OutputPrioritySplitter
        {
            public pos position;
            public int rotation;
            public bool output_priority;

            public OutputPrioritySplitter(pos position, int rotation)
            {
                this.position = position;
                this.rotation = rotation;
            }
            public OutputPrioritySplitter(pos position, int rotation, bool TFLeftOrRight)
            {
                this.position = position;
                this.rotation = rotation;
                this.output_priority = TFLeftOrRight;
            }

        }

        public class Belt
        {
            public pos position;
            public int rotation; // 0 = up, 2 = right, 4 = down, left = 6
            public List<object> connections = new List<object>();
            public bool hasSignal = false;
            public Belt(pos position, int rotation)
            {
                this.position = position;
                this.rotation = rotation;
            }
            public Belt(pos position, int rotation, object[] connections)
            {
                this.position = position;
                this.rotation = rotation;
                this.connections.AddRange(connections);
            }
        }

        public class roboport
        {
            public pos position;

            public roboport(pos position)
            {
                this.position = position;
            }
        }

        public class Substation
        {
            public pos position;
            public List<object> connections = new List<object>();
            public List<Substation> neighbors = new List<Substation>();

            public Substation(pos position)
            {
                this.position = position;
            }
        }

        public class InfinityChest
        {
            public pos position;
            public string item;
            public int count;
            public InfinityChest(pos position, string item, int count)
            {
                this.position = position;
                this.item = item;
                this.count = count;
            }
        }

        public class UnderGround
        {
            public pos position;
            public int rotation;
            public bool TFinputOrOutput; //input = true, output = false

            public UnderGround(pos position, int direction, bool TFinputOrOutput)
            {
                this.position = position;
                this.rotation = direction;
                this.TFinputOrOutput = TFinputOrOutput;
            }
        }

        public class PassiveProviderChest
        {
            public pos position;
            
            public PassiveProviderChest(pos position)
            {
                this.position = position;
            }
        }
        
        public static string GetDriverString(int width)
        {
            int Xmodifier = 320;
            int Ymodifier = -150 + ((width/2) * 10);
            object[] printerDriverComponents = new object[]
            {
                "{\"entity_number\":1,\"name\":\"constant-combinator\",\"position\":{\"x\":",
                -317.5 + Xmodifier,
                ",\"y\":",
                155.5 + Ymodifier,
                "},\"direction\":6,\"control_behavior\":{\"filters\":[{\"signal\":{\"type\":\"virtual\",\"name\":\"signal-dot\"},\"count\":",
                width.ToString(),
                ",\"index\":1}]},\"connections\":{\"1\":{\"red\":[{\"entity_id\":2,\"circuit_id\":1}]}}},{\"entity_number\":2,\"name\":\"arithmetic-combinator\",\"position\":{\"x\":",
                -315 + Xmodifier,
                ",\"y\":",
                155.5 + Ymodifier,
                "},\"direction\":2,\"control_behavior\":{\"arithmetic_conditions\":{\"first_signal\":{\"type\":\"virtual\",\"name\":\"signal-each\"},\"second_constant\":40,\"operation\":\"*\",\"output_signal\":{\"type\":\"virtual\",\"name\":\"signal-each\"}}},\"connections\":{\"1\":{\"red\":[{\"entity_id\":1},{\"entity_id\":8,\"circuit_id\":1}]},\"2\":{\"red\":[{\"entity_id\":3,\"circuit_id\":1}]}}},{\"entity_number\":3,\"name\":\"decider-combinator\",\"position\":{\"x\":",
                -313 + Xmodifier,
                ",\"y\":",
                155.5 + Ymodifier,
                "},\"direction\":2,\"control_behavior\":{\"decider_conditions\":{\"first_signal\":{\"type\":\"virtual\",\"name\":\"signal-T\"},\"second_signal\":{\"type\":\"virtual\",\"name\":\"signal-dot\"},\"comparator\":\"<\",\"output_signal\":{\"type\":\"virtual\",\"name\":\"signal-T\"},\"copy_count_from_input\":true}},\"connections\":{\"1\":{\"red\":[{\"entity_id\":2,\"circuit_id\":2},{\"entity_id\":4,\"circuit_id\":2}]},\"2\":{\"red\":[{\"entity_id\":4,\"circuit_id\":1},{\"entity_id\":10,\"circuit_id\":1}]}}},{\"entity_number\":4,\"name\":\"arithmetic-combinator\",\"position\":{\"x\":", 
                -313 + Xmodifier,
                " ,\"y\":",
                154.5 + Ymodifier,
                " },\"direction\":6,\"control_behavior\":{\"arithmetic_conditions\":{\"first_signal\":{\"type\":\"virtual\",\"name\":\"signal-T\"},\"second_constant\":1,\"operation\":\"+\",\"output_signal\":{\"type\":\"virtual\",\"name\":\"signal-T\"}}},\"connections\":{\"1\":{\"red\":[{\"entity_id\":3,\"circuit_id\":2}]},\"2\":{\"red\":[{\"entity_id\":3,\"circuit_id\":1}]}}},{\"entity_number\":5,\"name\":\"constant-combinator\",\"position\":{\"x\":", 
                -317.5 + Xmodifier,
                " ,\"y\":",
                156.5 + Ymodifier,
                " },\"direction\":6,\"control_behavior\":{\"filters\":[{\"signal\":{\"type\":\"virtual\",\"name\":\"signal-B\"},\"count\":3,\"index\":1}]},\"connections\":{\"1\":{\"red\":[{\"entity_id\":11}]}}},{\"entity_number\":6,\"name\":\"constant-combinator\",\"position\":{\"x\":", 
                -317.5 + Xmodifier,
                " ,\"y\":",
                157.5 + Ymodifier,
                " },\"direction\":6,\"control_behavior\":{\"filters\":[{\"signal\":{\"type\":\"virtual\",\"name\":\"signal-S\"},\"count\":2,\"index\":1}]},\"connections\":{\"1\":{\"red\":[{\"entity_id\":11}]}}},{\"entity_number\":7,\"name\":\"arithmetic-combinator\",\"position\":{\"x\":", 
                -315 + Xmodifier,
                " ,\"y\":",
                157.5 + Ymodifier,
                " },\"direction\":2,\"control_behavior\":{\"arithmetic_conditions\":{\"first_signal\":{\"type\":\"virtual\",\"name\":\"signal-each\"},\"second_constant\":10,\"operation\":\"+\",\"output_signal\":{\"type\":\"virtual\",\"name\":\"signal-each\"}}},\"connections\":{\"1\":{\"red\":[{\"entity_id\":8,\"circuit_id\":2}]},\"2\":{\"red\":[{\"entity_id\":9,\"circuit_id\":1}]}}},{\"entity_number\":8,\"name\":\"arithmetic-combinator\",\"position\":{\"x\":", 
                -315 + Xmodifier,
                " ,\"y\":",
                156.5 + Ymodifier,
                " },\"direction\":2,\"control_behavior\":{\"arithmetic_conditions\":{\"first_signal\":{\"type\":\"virtual\",\"name\":\"signal-each\"},\"second_constant\":3,\"operation\":\"*\",\"output_signal\":{\"type\":\"virtual\",\"name\":\"signal-each\"}}},\"connections\":{\"1\":{\"red\":[{\"entity_id\":2,\"circuit_id\":1}]},\"2\":{\"red\":[{\"entity_id\":7,\"circuit_id\":1}]}}},{\"entity_number\":9,\"name\":\"decider-combinator\",\"position\":{\"x\":", 
                -313 + Xmodifier,
                " ,\"y\":",
                157.5 + Ymodifier,
                " },\"direction\":2,\"control_behavior\":{\"decider_conditions\":{\"first_signal\":{\"type\":\"virtual\",\"name\":\"signal-T\"},\"second_signal\":{\"type\":\"virtual\",\"name\":\"signal-dot\"},\"comparator\":\"<\",\"output_signal\":{\"type\":\"virtual\",\"name\":\"signal-D\"},\"copy_count_from_input\":false}},\"connections\":{\"1\":{\"red\":[{\"entity_id\":7,\"circuit_id\":2},{\"entity_id\":10,\"circuit_id\":1}]},\"2\":{\"red\":[{\"entity_id\":10,\"circuit_id\":2},{\"entity_id\":11}]}}},{\"entity_number\":10,\"name\":\"decider-combinator\",\"position\":{\"x\":", 
                -313 + Xmodifier,
                " ,\"y\":",
                156.5 + Ymodifier,
                " },\"direction\":2,\"control_behavior\":{\"decider_conditions\":{\"first_signal\":{\"type\":\"virtual\",\"name\":\"signal-T\"},\"constant\":15,\"comparator\":\"<\",\"output_signal\":{\"type\":\"virtual\",\"name\":\"signal-A\"},\"copy_count_from_input\":false}},\"connections\":{\"1\":{\"red\":[{\"entity_id\":3,\"circuit_id\":2},{\"entity_id\":9,\"circuit_id\":1}]},\"2\":{\"red\":[{\"entity_id\":9,\"circuit_id\":2}]}}},{\"entity_number\":11,\"name\":\"substation\",\"position\":{\"x\":", 
                -311 + Xmodifier,
                " ,\"y\":",
                157 + Ymodifier,
                " },\"connections\":{\"1\":{\"red\":[{\"entity_id\":6},{\"entity_id\":9,\"circuit_id\":2},{\"entity_id\":5}]}}},"
            };

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < printerDriverComponents.Length; i++)
            {
                if(printerDriverComponents[i] != null)
                {
                    
                    sb.Append(printerDriverComponents[i].ToString());
                    
                }
            }
            
            
            return sb.ToString();
        }
    }

    /// <summary>
    /// Class for organizing blueprint components
    /// not complete to the full spec, but contains everything
    /// needed for this project
    /// </summary>
    public class Blueprint
    {
        /// <summary>
        /// not sure what this is for but if it needs to be changed, its right here
        /// </summary>
        public string version = "281479274168320";






        /// <summary>
        /// Main class containing all information needed for a single entity
        /// as well as any components that are contained with it
        /// </summary>
        public class entity
        {
            public static int lastEntityNumber = 11;  //start with 11 so I can insert all the combinators and logic before without having to define it here

            public string name;
            public int entity_number;

            public List<entityComponent> components = new List<entityComponent>();

            public entity(string name)
            {
                this.name = name;
                lastEntityNumber++;
                entity_number = lastEntityNumber;
            }
            public entity(string name, entityComponent[] components)
            {
                this.name = name;
                this.components.AddRange(components);
                lastEntityNumber++;
                entity_number = lastEntityNumber;
            }

            public int GetEntityNumber()
            {
                return entity_number;
            }

            public string getJson()
            {
                string header = String.Concat(new string[] {
                    "{ \"entity_number\": ",
                    GetEntityNumber().ToString(),
                    ",\"name\": \"",
                    name,
                    "\""
                });

                string body = "";

                if (components.Count > 0)
                {
                    body += ",";
                    for (int i = 0; i < components.Count; i++)
                    {
                        entityComponent entity_component = components[i];
                        body += entity_component.getJson();
                        if (i < components.Count - 1)
                        {
                            body += ",";
                        }
                    }
                }

                string footer = "}";

                return header + body + footer;
            }
        }

        /// <summary>
        /// Collection of multiple entities that can be repeated for easier building
        /// </summary>
        public class entityGroup
        {
            public float x;
            public float y;
            public List<entity> entities = new List<entity>();
        }


        public class entityComponent
        {
            public string componentName = ""; //name of the component eg. "position"

            public virtual string getJson() //returns the string that represents this component in a json file
            {
                return "emptyComponent";
            }
        }


        /// <summary>
        /// Item requested by requester chests
        /// </summary>
        public class request_filters : entityComponent
        {
            string request_item;
            int count;
            public request_filters(string item, int count)
            {
                request_item = item;
                this.count = count;
                this.componentName = "request_filters";

            }

            public override string getJson()
            {
                return "\"" + componentName + "\": [{\"index\": 1, \"name\": \"" + request_item + "\",\"count\": " + count + "}]";
            }

        }

        public class position : entityComponent
        {
            float x;
            float y;

            public position(float x, float y)
            {
                this.x = x;
                this.y = y;
                this.componentName = "position";
            }

            public override string getJson()
            {
                return "\"" + componentName + "\": {\"x\": " + x + ",\"y\": " + y + "}";
            }
        }

        public class direction : entityComponent
        {
            int componentDirection;

            public direction(int direction)
            {
                this.componentDirection = direction;
                this.componentName = "direction";
            }

            public override string getJson()
            {
                return "\"" + componentName + "\": " + componentDirection;
            }
        }

        /// <summary>
        /// defines wire connections to other entities by their entity_number,
        /// limited to connection point 1, and only to red wires
        /// </summary>
        public class connections : entityComponent
        {
            public List<int> entity_numbers = new List<int>();
            public connections(int[] entity_numbers)
            {
                this.entity_numbers.AddRange(entity_numbers);
            }

            public void AddConnection(int entity_number)
            {
                entity_numbers.Add(entity_number);
            }

            public override string getJson()
            {
                string header = "\"connections\": { \"1\": {\"red\": [";
                string body = "";
                for (int i = 0; i < entity_numbers.Count; i++)
                {
                    int ID = entity_numbers[i];
                    body += "{\"entity_id\": " + ID + "}";

                    if (i < entity_numbers.Count - 1)
                    {
                        body += ",";
                    }
                }
                string footer = "]}}";
                return header + body + footer;
            }
        }
        /// <summary>
        /// Just a preset control behavior for inserters using the only circut conditions that i need
        /// </summary>
        public class insertercontrolbehavior : entityComponent
        {
            public insertercontrolbehavior()
            {
                this.componentName = "control_behavior";
            }
            public override string getJson()
            {
                return "\"control_behavior\": {\"circuit_condition\": {\"first_signal\": {\"type\": \"virtual\",\"name\": \"signal-A\"},\"constant\": 0,\"comparator\": \"\u003E\"},\"circuit_set_stack_size\": true,\"stack_control_input_signal\": { \"type\": \"virtual\",\"name\": \"signal-S\"}}";
            }

        }

        public class neighbors : entityComponent
        {
            public List<int> entity_numbers = new List<int>();

            public neighbors(int[] neighbors)
            {
                entity_numbers.AddRange(neighbors);
            }

            public override string getJson()
            {
                string head = "\"neighbours\": [";
                string body = "";
                for (int i = 0; i < entity_numbers.Count; i++)
                {

                    body += entity_numbers[i].ToString();

                    if (i < entity_numbers.Count - 1)
                    {
                        body += ",";
                    }
                }
                string tail = "]";

                return head + body + tail;
            }
        }

        public class infinity_settings : entityComponent
        {
            string item;
            int count;
            public infinity_settings(string item, int count)
            {
                this.item = item;
                this.count = count;
                this.componentName = "infinity_settings";

            }

            public override string getJson()
            {
                return "\"" + componentName + "\": {\"remove_unfiltered_items\": false,\"filters\": [{\"name\": \"" + item + "\",\"count\": " + count.ToString() + ",\"mode\": \"at-least\",\"index\": 1}]}";


            }
        }

        public class input_priority : entityComponent
        {
            string leftOrRight;
            public input_priority(string leftOrRight)
            {
                this.leftOrRight = leftOrRight;
                componentName = "input_priority";
            }

            public override string getJson()
            {
                return "\"" + componentName + "\": \"" + leftOrRight + "\"";
            }
        }

        public class output_priority : entityComponent
        {
            string leftOrRight;
            public output_priority(string leftOrRight)
            {
                this.leftOrRight = leftOrRight;
                componentName = "output_priority";
            }

            public override string getJson()
            {
                return "\"" + componentName + "\": \"" + leftOrRight + "\"";
            }
        }

        /// <summary>
        /// preset control behavior for belts to act as the gate
        /// </summary>
        public class beltControlBehavior : entityComponent
        {
            public beltControlBehavior()
            {
                this.componentName = "control_behavior";
            }

            public override string getJson()
            {
                return "\"control_behavior\": { \"circuit_condition\": {\"first_signal\": { \"type\": \"virtual\",\"name\": \"signal-D\" }, \"constant\": 1, \"comparator\": \"\\u2265\" }, \"circuit_enable_disable\": true, \"circuit_read_hand_contents\": false,\"circuit_contents_read_mode\": 0 }";
            }
        }

        public class type : entityComponent
        {
            bool TFInputOrOutput;
            public type(bool TFInputOrOutput)
            {
                this.componentName = "type";
                this.TFInputOrOutput = TFInputOrOutput;
            }

            public override string getJson()
            {
                return "\"" + componentName + "\": \"" + (TFInputOrOutput ? "input" : "output") + "\"";
            }
        }

        public class control_behavior2 : entityComponent
        {
            public override string getJson()
            {
                return "\"control_behavior\": {\"arithmetic_conditions\": {\"first_signal\": {\"type\": \"virtual\",\"name\": \"signal-each\"},\"second_constant\": 40,\"operation\": \"*\",\"output_signal\": {\"type\": \"virtual\",\"name\": \"signal-each\"}}}";
            }
        }

        public class control_behavior3 : entityComponent
        {
            public override string getJson()
            {
                return "\"control_behavior\": {\"decider_conditions\": {\"first_signal\": {\"type\": \"virtual\",\"name\": \"signal-T\"},\"second_signal\": {\"type\": \"virtual\",\"name\": \"signal-dot\"},\"comparator\": \"\\u003C\",\"output_signal\": {\"type\": \"virtual\",\"name\": \"signal-T\"},\"copy_count_from_input\": true}}";
            }
        }

        public class Combinatorconnections : entityComponent
        {
            public List<int> entity_numbers1 = new List<int>();
            public List<int> entity_numbers2 = new List<int>();
            public Combinatorconnections(int[] circut1_numbers, int[] circut2_numbers)
            {
                this.entity_numbers1.AddRange(entity_numbers1);
                this.entity_numbers2.AddRange(entity_numbers2);
            }

            public void AddConnection(int entity_number, int circut)
            {
                if (circut == 1)
                    entity_numbers1.Add(entity_number);
                if (circut == 2)
                    entity_numbers2.Add(entity_number);
            }

            public override string getJson()
            {
                string header = "\"connections\": { \"1\": {\"red\": [";
                string body = "";
                for (int i = 0; i < entity_numbers1.Count; i++)
                {
                    int ID = entity_numbers1[i];
                    body += "{\"entity_id\": " + ID + "}";

                    if (i < entity_numbers1.Count - 1)
                    {
                        body += ",";
                    }
                }
                string footer = "]}}";
                return header + body + footer;
            }
        }
    }



}
