using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Security.Cryptography;

namespace hackerWorldNS
{
    enum hex { A = 10, B, C, D, E, F }

    class ComboList
    {
        private bool visible;
        private bool open;
        private bool enabled;
        private List<string> items;
        private int selectedItem;
        private Vector2 location;
        private Texture2D boxTexture;
        private Texture2D openTexture;
        private SpriteFont font;
        private int scrollPos;

        public bool Visible { get { return visible; } set { visible = value; } }
        public bool Open { get { return open; } set { open = value; } }
        public bool Enabled { get { return enabled; } set { enabled = value; } }
        public List<string> Items { get { return items; } set { items = value; } }
        public int SelectedItem { get { return selectedItem; } set { selectedItem = value; } }
        public Vector2 Location { get { return location; } set { location = value; } }
        public Texture2D BoxTexture { get { return boxTexture; } set { boxTexture = value; } }
        public Texture2D OpenTexture { get { return openTexture; } set { openTexture = value; } }
        public SpriteFont Font { get { return font; } set { font = value; } }

        public ComboList(List<string> itemList, ref Texture2D boxTexture, ref Texture2D openTexture, ref SpriteFont font, Vector2 location)
        {
            this.items = itemList;
            this.selectedItem = 0;
            this.boxTexture = boxTexture;
            this.openTexture = openTexture;
            this.font = font;
            this.location = location;
            this.enabled = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Begin();
            if (!enabled)
            {
                spriteBatch.Draw(boxTexture, location, Color.White);
                spriteBatch.DrawString(font, "Disabled",
                                       new Vector2(location.X + 13, location.Y + 3), Color.Black);
            }
            else if (!open)
            {
                spriteBatch.Draw(boxTexture, location, Color.White);
                spriteBatch.DrawString(font, items[selectedItem],
                                       new Vector2(location.X + 13, location.Y + 3), Color.Black);
            }
            else
            {
                spriteBatch.Draw(openTexture, location, Color.White);
                spriteBatch.DrawString(font, items[selectedItem],
                                       new Vector2(location.X + 13, location.Y + 3), Color.Black);
                int pos = 1;
                for (int i = scrollPos; i < scrollPos + 3; i++)
                {
                    if (i >= items.Count || i < 0)
                        break;
                    spriteBatch.DrawString(font, items[i],
                                       new Vector2(location.X + 13, (13 * pos) + location.Y + 6), Color.Black);
                    pos++;
                }
            }
            //spriteBatch.End();
        }

        public void Click(Vector2 position)
        {
            //box is diabled
            if (!enabled)
                return;
            //open pulldown
            if (position.X >= location.X + boxTexture.Width - 20 && position.X < location.X + boxTexture.Width &&
               position.Y >= location.Y && position.Y <= location.Y + boxTexture.Height)
                this.open = !this.open;
            //scroll down
            else if (open && scrollPos < items.Count - 3 &&
               position.X >= location.X + openTexture.Width - 20 && position.X < location.X + openTexture.Width - 5 &&
               position.Y >= location.Y + openTexture.Height - 15 && position.Y <= location.Y + openTexture.Height)
                this.scrollPos++;
            //scroll up
            else if (open && scrollPos > 0 &&
               position.X >= location.X + openTexture.Width - 20 && position.X < location.X + openTexture.Width - 5 &&
               position.Y >= location.Y + 20 && position.Y <= location.Y + 35)
                this.scrollPos--;
            //select entry 1st
            else if (open && items.Count > this.scrollPos &&
               position.X >= location.X + 10 && position.X < location.X + openTexture.Width - 17 &&
               position.Y >= location.Y + 20 && position.Y <= location.Y + 33)
            {
                this.selectedItem = this.scrollPos;
                this.open = false;
            }
            //select entry 2nd
            else if (open && items.Count > this.scrollPos + 1 &&
               position.X >= location.X + 10 && position.X < location.X + openTexture.Width - 17 &&
               position.Y >= location.Y + 33 && position.Y <= location.Y + 46)
            {
                this.selectedItem = this.scrollPos + 1;
                this.open = false;
            }//select entry 3rd
            else if (open && items.Count > this.scrollPos + 2 &&
               position.X >= location.X + 10 && position.X < location.X + openTexture.Width - 17 &&
               position.Y >= location.Y + 46 && position.Y <= location.Y + 59)
            {
                this.selectedItem = this.scrollPos + 2;
                this.open = false;
            }
        }
    }

    class RightClickMenu
    {
        private List<Texture2D> menuItemsTextures = new List<Texture2D>();
        private List<Texture2D> menuItemsSelected = new List<Texture2D>();
        private List<Vector2> menuItemsLocations = new List<Vector2>();
        private Vector2 menuPosition = new Vector2();
        private bool open = false;

        public List<Texture2D> MenuItemsTextures { get { return menuItemsTextures; } set { menuItemsTextures = value; } }
        public List<Texture2D> MenuItemsSelected { get { return menuItemsSelected; } set { menuItemsSelected = value; } }
        public List<Vector2> MenuItemsLocations { get { return menuItemsLocations; } set { menuItemsLocations = value; } }
        public bool Open { get { return open; } set { open = value; } }

        /// <summary>
        /// default constructor
        /// </summary>
        public RightClickMenu() { }

        /// <summary>
        /// add item to menu
        /// </summary>
        /// <param name="image">image</param>
        /// <param name="imageSelected">image on mouse over</param>
        /// <param name="imageLocation">relative location</param>
        public void addMenuItem(Texture2D image, Texture2D imageSelected, Vector2 imageLocation)
        {
            menuItemsTextures.Add(image);
            menuItemsSelected.Add(imageSelected);
            menuItemsLocations.Add(imageLocation);
        }

        /// <summary>
        /// set menu location
        /// </summary>
        /// <param name="location"></param>
        public void setMenuLocation(Vector2 location)
        {
            menuPosition = location;
        }

        /// <summary>
        /// draw menu
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="mousePos"></param>
        public int drawMenu(SpriteBatch spriteBatch, Point mousePos)
        {
            //menu is not visible
            if (!open)
                return -1;
            int tmp = -1;

            //spriteBatch.Begin();
            for (int i = 0; i < menuItemsLocations.Count; i++)
            {
                if (mousePos.X >= menuItemsLocations[i].X + menuPosition.X && mousePos.X <= menuItemsLocations[i].X + menuPosition.X + menuItemsTextures[i].Width &&
                    mousePos.Y >= menuItemsLocations[i].Y + menuPosition.Y && mousePos.Y <= menuItemsLocations[i].Y + menuPosition.Y + menuItemsTextures[i].Height)
                {
                    Vector2 tmpLoc = new Vector2(menuItemsLocations[i].X + menuPosition.X, menuItemsLocations[i].Y + menuPosition.Y);
                    spriteBatch.Draw(menuItemsSelected[i], tmpLoc, Color.White);
                    tmp = i;
                }
                else
                {
                    Vector2 tmpLoc = new Vector2(menuItemsLocations[i].X + menuPosition.X, menuItemsLocations[i].Y + menuPosition.Y);
                    spriteBatch.Draw(menuItemsTextures[i], tmpLoc, Color.White);
                }
            }
            //spriteBatch.End();
            return tmp;
        }

        public int click(Point mousePos)
        {
            //menu is not visible
            if (!open)
                return -2;

            for (int i = 0; i < menuItemsLocations.Count; i++)
            {
                if (mousePos.X >= menuItemsLocations[i].X + menuPosition.X && mousePos.X <= menuItemsLocations[i].X + menuPosition.X + menuItemsTextures[i].Width &&
                    mousePos.Y >= menuItemsLocations[i].Y + menuPosition.Y && mousePos.Y <= menuItemsLocations[i].Y + menuPosition.Y + menuItemsTextures[i].Height)
                {
                    return i;
                }
            }

            return -1;
        }
    }

    /// <summary>
    /// basic window class that accepts components as objects
    /// </summary>
    class Window
    {
        private bool open = false;
        private bool moving = false;
        private bool floating = true;
        private bool focus = false;
        private bool slave = false;
        private Vector2 position;
        private Vector2 moveOffset;
        private Texture2D backTexture;
        private Point size;
        private Color backColor;
        private SpriteFont font;
        private string title;
        private string name;
        private List<Object> objects = new List<Object>();

        public bool Open { get { return open; } set { open = value; } }
        public bool Floating { get { return floating; } set { floating = value; } }
        public bool Moving { get { return moving; } set { moving = value; } }
        public bool Focus { get { return focus; } set { focus = value; } }
        public bool Slave { get { return slave; } set { slave = value; } }
        public string Title { get { return title; } set { title = value; } }
        public string Name { get { return name; } set { name = value; } }
        public Vector2 Position { get { return position; } set { position = value; } }
        public Texture2D BackTexture { get { return backTexture; } set { backTexture = value; } }
        public Point Size { get { return size; } set { size = value; } }
        public Color BackColor { get { return backColor; } set { backColor = value; } }
        public SpriteFont Font { get { return font; } set { font = value; } }
        public event EventHandler<SimpleEventArgs> RaiseClearFocus;
        public event EventHandler<CustomDrawEventArgs> RaiseCustomDraw;

        public Window(){}
        public Window(Texture2D blankTexture, Vector2 position, Point size, Color backColor, SpriteFont font, String title)
        {
            this.backTexture = blankTexture;
            this.position = position;
            this.size = size;
            this.backColor = backColor;
            this.font = font;
            this.title = title;
            this.name = title;
        }

        public void addObject(Object child)
        {
            if (child.GetType() == typeof(TextBox))
            {
                TextBox textBox = (TextBox)child;
                objects.Add(textBox);
            }
            else if (child.GetType() == typeof(Label))
            {
                Label label = (Label)child;
                objects.Add(label);
            }
            else if (child.GetType() == typeof(Button))
            {
                Button button = (Button)child;
                objects.Add(button);
            }
            else if (child.GetType() == typeof(DropBox))
            {
                DropBox dropBox = (DropBox)child;
                dropBox.Parent = this.name;
                objects.Add(dropBox);
            }
            else if (child.GetType() == typeof(Window))
            {
                Window window = (Window)child;
                //window.Parent = this.name;
                objects.Add(window);
            }
            else if (child.GetType() == typeof(ProgressBar))
            {
                ProgressBar pBar = (ProgressBar)child;
                pBar.Parent = this.name;
                objects.Add(pBar);
            }
        }

        public Object getObjectByName(string name)
        {
            foreach (Object ob in objects)
                if (ob.GetType() == typeof(TextBox) && ((TextBox)ob).Name == name)
                    return ob;
                else if (ob.GetType() == typeof(Label) && ((Label)ob).Name == name)
                    return ob;
                else if (ob.GetType() == typeof(DropBox) && ((DropBox)ob).Name == name)
                    return ob;
                else if (ob.GetType() == typeof(Button) && ((Button)ob).Name == name)
                    return ob;
                else if (ob.GetType() == typeof(Window) && ((Window)ob).Name == name)
                    return ob;
                else if (ob.GetType() == typeof(ProgressBar) && ((ProgressBar)ob).Name == name)
                    return ob;

            return null;
        }

        public void getNextControl(string name)
        {
            clearActive();
            int id = 0;
            foreach (Object ob in objects)
            {
                id++;
                if (ob.GetType() == typeof(TextBox) && ((TextBox)ob).Name == name)
                    for (int i = id; i < objects.Count; i++)
                        if (objects[i].GetType() == typeof(TextBox))
                        {
                            ((TextBox)objects[i]).Active = true;
                            return;
                        }
            }
        }

        public Object getActiveObject()
        {
            foreach (Object ob in objects)
                if (ob.GetType() == typeof(TextBox) && ((TextBox)ob).Active)
                    return ob;
            return null;
        }

        public void clearActive()
        {
            foreach (Object ob in objects)
                if (ob.GetType() == typeof(TextBox))
                    ((TextBox)ob).Active = false;
        }

        public void mouseDown(MouseState mouseState)
        {
            if (!open)
                return;

            //is it in my bounds
            if (!(mouseState.X >= position.X && mouseState.X <= position.X + size.X &&
                mouseState.Y >= position.Y && mouseState.Y <= position.Y + size.Y) && !moving)
                return;

            if (!focus)
            {
                OnRaiseCleanFocusEvent(new SimpleEventArgs(""));
                focus = true;
            }

            Vector2 oldPos = this.position;
            Vector2 newPos = new Vector2();
            //move window
            if (moving && floating && !slave)
            {
                newPos = new Vector2(mouseState.X + moveOffset.X, mouseState.Y + moveOffset.Y);
                this.position = newPos;
            }

            //move all slave windows
            if (moving)
            {
                foreach (Object ob in objects)
                    if (ob.GetType() == typeof(Window))
                    {
                        Window tmpWin = (Window)ob;
                        tmpWin.position.X += newPos.X - oldPos.X;
                        tmpWin.position.Y += newPos.Y - oldPos.Y;
                    }
            }

            //set move to active
            if (mouseState.X >= position.X && mouseState.X <= position.X + size.X &&
                mouseState.Y >= position.Y && mouseState.Y <= position.Y + 25)
            {
                if (!moving && !slave)
                    moveOffset = new Vector2(position.X - mouseState.X, position.Y - mouseState.Y);
                
                moving = true;
            }
            //cycyle trough objects that need mouseDown event
            else
            {
                Vector2 relativePos = new Vector2(mouseState.X - position.X,mouseState.Y - position.Y);
                foreach(Object ob in objects)
                    if (ob.GetType() == typeof(Button))
                    {
                        Button tmpOb = (Button)ob;
                        if (relativePos.X >= tmpOb.Position.X && relativePos.X <= tmpOb.Position.X + tmpOb.Size.X &&
                            relativePos.Y >= tmpOb.Position.Y && relativePos.Y <= tmpOb.Position.Y + tmpOb.Size.Y)
                            tmpOb.onMouseDown();
                    }
                    else if (ob.GetType() == typeof(DropBox))
                    {
                        DropBox tmpOb = (DropBox)ob;
                        if (relativePos.X >= tmpOb.Position.X && relativePos.X <= tmpOb.Position.X + tmpOb.Size.X &&
                            relativePos.Y >= tmpOb.Position.Y && relativePos.Y <= tmpOb.Position.Y + tmpOb.Size.Y)
                             tmpOb.onMouseDown();
                    }
                    else if (ob.GetType() == typeof(Window))
                    {
                        Window tmpOb = (Window)ob;
                        if (mouseState.X >= tmpOb.Position.X && mouseState.X <= tmpOb.Position.X + tmpOb.Size.X &&
                            mouseState.Y >= tmpOb.Position.Y && mouseState.Y <= tmpOb.Position.Y + tmpOb.Size.Y)
                            tmpOb.mouseDown(mouseState);
                    }
            }
        }

        public void mouseUp(MouseState mouseState)
        {
            if (!this.open)
                return;

            this.moving = false;

            Vector2 relativePos = new Vector2(mouseState.X - position.X,mouseState.Y - position.Y);
            //go trough objects that need mouse up
            for (int i = 0; i < objects.Count; i++)
            {
                Object ob = objects[i];
                if (ob.GetType() == typeof(Button))
                {
                    Button tmpOb = (Button)ob;
                    if (relativePos.X >= tmpOb.Position.X && relativePos.X <= tmpOb.Position.X + tmpOb.Size.X &&
                        relativePos.Y >= tmpOb.Position.Y && relativePos.Y <= tmpOb.Position.Y + tmpOb.Size.Y)
                        tmpOb.onMouseUp();
                }
                else if (ob.GetType() == typeof(DropBox))
                {
                    DropBox tmpOb = (DropBox)ob;
                    if (relativePos.X >= tmpOb.Position.X && relativePos.X <= tmpOb.Position.X + tmpOb.Size.X &&
                        relativePos.Y >= tmpOb.Position.Y && relativePos.Y <= tmpOb.Position.Y + tmpOb.Size.Y)
                        tmpOb.onMouseUp();
                }
                else if (ob.GetType() == typeof(Window))
                {
                    Window tmpOb = (Window)ob;
                    if (mouseState.X >= tmpOb.Position.X && mouseState.X <= tmpOb.Position.X + tmpOb.Size.X &&
                        mouseState.Y >= tmpOb.Position.Y && mouseState.Y <= tmpOb.Position.Y + tmpOb.Size.Y)
                        tmpOb.mouseUp(mouseState);
                }
            }
        }

        public void click(Vector2 mousePos)
        {
            if (!open)
                return;

            //close region clicked
            if (mousePos.X >= position.X + size.X - 12 && mousePos.X <= position.X + size.X &&
                mousePos.Y >= position.Y && mousePos.Y <= position.Y + 15 && !this.slave)
            {
                open = false;
                this.moving = false;
                return;
            }

            //convert to local coords
            mousePos.X -= position.X;
            mousePos.Y -= position.Y;

            foreach (Object ob in objects)
                if (ob.GetType() == typeof(TextBox))
                {
                    TextBox tmpOb = (TextBox)ob;
                    if (mousePos.X >= tmpOb.Position.X && mousePos.X <= tmpOb.Position.X + tmpOb.Size.X &&
                       mousePos.Y >= tmpOb.Position.Y && mousePos.Y <= tmpOb.Position.Y + tmpOb.Size.Y)
                    {
                        clearActive();
                        tmpOb.Active = true;
                    }
                }
                else if (ob.GetType() == typeof(Button))
                {
                    Button tmpOb = (Button)ob;
                    if (mousePos.X >= tmpOb.Position.X && mousePos.X <= tmpOb.Position.X + tmpOb.Size.X &&
                       mousePos.Y >= tmpOb.Position.Y && mousePos.Y <= tmpOb.Position.Y + tmpOb.Size.Y)
                    {
                        clearActive();
                        tmpOb.click();
                    }
                }
                else if (ob.GetType() == typeof(DropBox))
                {
                    DropBox tmpOb = (DropBox)ob;
                    if (mousePos.X >= tmpOb.Position.X && mousePos.X <= tmpOb.Position.X + tmpOb.Size.X &&
                       mousePos.Y >= tmpOb.Position.Y && mousePos.Y <= tmpOb.Position.Y + tmpOb.Size.Y)
                    {
                        clearActive();
                        tmpOb.click(mousePos);
                    }
                }
                else if (ob.GetType() == typeof(Window))
                {
                    mousePos.X += position.X;
                    mousePos.Y += position.Y;

                    Window tmpOb = (Window)ob;
                    if (mousePos.X >= tmpOb.Position.X && mousePos.X <= tmpOb.Position.X + tmpOb.Size.X &&
                        mousePos.Y >= tmpOb.Position.Y && mousePos.Y <= tmpOb.Position.Y + tmpOb.Size.Y)
                    {
                        clearActive();
                        tmpOb.click(mousePos);
                    }
                }
        }

        public void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)
        {
            if (!open)
                return;

            spriteBatch.Begin();
            spriteBatch.Draw(backTexture, new Rectangle((int)position.X, (int)position.Y, size.X, size.Y), backColor);
            Vector2 tmpPos = new Vector2(position.X + size.X / 2 - title.Length * 6 / 2, position.Y + 3);
            spriteBatch.DrawString(font, title, tmpPos, Color.Black);
            tmpPos = new Vector2(position.X + size.X - 12,position.Y + 3);
            spriteBatch.DrawString(font, "X", tmpPos, Color.Black);
            spriteBatch.End();

            primitiveBatch.Begin(PrimitiveType.LineList);
            primitiveBatch.AddVertex(new Vector2(position.X, position.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(position.X + size.X, position.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(position.X, position.Y + 20), Color.Black);
            primitiveBatch.AddVertex(new Vector2(position.X + size.X, position.Y + 20), Color.Black);
            primitiveBatch.AddVertex(new Vector2(position.X + size.X, position.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(position.X + size.X, position.Y + size.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(position.X + size.X, position.Y + size.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(position.X, position.Y + size.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(position.X, position.Y + size.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(position.X, position.Y), Color.Black);
            primitiveBatch.End();

            //draw all objects part of this window
            foreach (Object ob in objects)
                if (ob.GetType() == typeof(TextBox))
                    ((TextBox)ob).Draw(spriteBatch, primitiveBatch, this.position);
                else if (ob.GetType() == typeof(Label))
                    ((Label)ob).Draw(spriteBatch, primitiveBatch, this.position);
                else if (ob.GetType() == typeof(Button))
                    ((Button)ob).Draw(spriteBatch, primitiveBatch, this.position);
                else if (ob.GetType() == typeof(DropBox))
                    ((DropBox)ob).Draw(spriteBatch, primitiveBatch, this.position);
                else if (ob.GetType() == typeof(Window))
                    ((Window)ob).Draw(spriteBatch, primitiveBatch);
                else if (ob.GetType() == typeof(ProgressBar))
                    ((ProgressBar)ob).Draw(spriteBatch, primitiveBatch, this.position);

            OnRaiseCustomDrawEvent(new CustomDrawEventArgs(this.name, this.position));
        }

        public void dumpObjects()
        {
            objects.Clear();
        }

        protected virtual void OnRaiseCleanFocusEvent(SimpleEventArgs e)
        {
            EventHandler<SimpleEventArgs> handler = RaiseClearFocus;

            if (handler != null)
            {
                e.Message = this.name;
                handler(this, e);
            }
        }

        protected virtual void OnRaiseCustomDrawEvent(CustomDrawEventArgs e)
        {
            EventHandler<CustomDrawEventArgs> handler = RaiseCustomDraw;

            if (handler != null)
                handler(this, e);
        }
    }

    class ProgressBar
    {
        private string name = "";
        private string parent = "";
        private long target;
        private long value;
        private bool hasBorder;
        private Color backColor;
        private Color foreColor;
        private Vector2 position;
        private Vector2 size = new Vector2(50, 10);
        private Texture2D texture;

        public string Name { get { return name; } set { name = value; } }
        public string Parent { get { return parent; } set { parent = value; } }
        public long Target { get { return target; } set { target = value; } }
        public long Value { get { return value; } set { this.value = value; } }
        public bool HasBorder { get { return hasBorder; } set { hasBorder = value; } }
        public Vector2 Position { get { return position; } set { position = value; } }
        public Vector2 Size { get { return size; } set { size = value; } }
        public Color BackColor { get { return backColor; } set { backColor = value; } }
        public Color ForeColor { get { return foreColor; } set { foreColor = value; } }
        public Texture2D Texture { get { return texture; } set { texture = value; } }

        public ProgressBar(string name, Vector2 location, Vector2 size, Color backColor, Color foreColor, Texture2D texture)
        {
            this.name = name;
            this.position = location;
            this.backColor = backColor;
            this.foreColor = foreColor;
            this.size = size;
            this.value = 0;
            this.target = 100;
            this.texture = texture;
        }

        public void setValues(long target, long value)
        {
            this.target = target;
            this.value = value;
            if (value > target)
                this.value = this.target;
        }

        public void makeStep()
        {
            if (this.value < this.target)
                this.value++;
        }

        public void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch, Vector2 parentPos)
        {
            Vector2 parentOffset = new Vector2(parentPos.X + position.X, parentPos.Y + position.Y);

            //draw border
            if (hasBorder)
            {
                primitiveBatch.Begin(PrimitiveType.LineList);
                primitiveBatch.AddVertex(new Vector2(parentOffset.X - 1, parentOffset.Y - 1), Color.Black);//top left
                primitiveBatch.AddVertex(new Vector2(parentOffset.X + size.X + 1, parentOffset.Y - 1), Color.Black);//top right
                primitiveBatch.AddVertex(new Vector2(parentOffset.X + size.X + 1, parentOffset.Y - 1), Color.Black);//top right
                primitiveBatch.AddVertex(new Vector2(parentOffset.X + size.X + 1, parentOffset.Y + size.Y + 1), Color.Black);//bottom right
                primitiveBatch.AddVertex(new Vector2(parentOffset.X + size.X + 1, parentOffset.Y + size.Y + 1), Color.Black);//bottom right
                primitiveBatch.AddVertex(new Vector2(parentOffset.X - 1, parentOffset.Y + size.Y + 1), Color.Black);//bottom left
                primitiveBatch.AddVertex(new Vector2(parentOffset.X - 1, parentOffset.Y + size.Y + 1), Color.Black);//bottom left
                primitiveBatch.AddVertex(new Vector2(parentOffset.X - 1, parentOffset.Y - 1), Color.Black);//top left
                primitiveBatch.End();
            }

            //draw texture
            spriteBatch.Begin();
            if (hasBorder)
                spriteBatch.Draw(texture, new Rectangle((int)parentOffset.X, (int)parentOffset.Y, (int)size.X, (int)size.Y), backColor);

            Vector2 fillLevel = new Vector2(0, this.size.Y);
            fillLevel.X = this.size.X * (this.value / (this.target + 0.1f));

            //draw progress part
            spriteBatch.Draw(texture, new Rectangle((int)parentOffset.X, (int)parentOffset.Y, (int)fillLevel.X, (int)fillLevel.Y), foreColor);
            spriteBatch.End();
        }
    }

    class DropBox
    {
        private string name = "";
        private string parent = "";
        private string holding = "";
        private Texture2D texture;
        private Texture2D backTexture;
        private Vector2 position;
        private Point size = new Point(24, 24);
        private Color backColor;
        private bool canDrag = true;
        private bool hasBorder = true;
        private bool visible = true;

        public string Holding { get { return holding; } set { holding = value; } }
        public string Name { get { return name; } set { name = value; } }
        public string Parent { get { return parent; } set { parent = value; } }
        public bool CanDrag { get { return canDrag; } set { canDrag = value; } }
        public bool HasBorder { get { return hasBorder; } set { hasBorder = value; } }
        public bool Visible { get { return visible; } set { visible = value; } }
        public Texture2D Texture { get { return texture; } set { texture = value; } }
        public Texture2D BackTexture { get { return backTexture; } set { backTexture = value; } }
        public Vector2 Position { get { return position; } set { position = value; } }
        public Color BackColor { get { return backColor; } set { backColor = value; } }
        public Point Size { get { return size; } set { size = value; } }
        public event EventHandler<SimpleEventArgs> RaiseButtonClick;
        public event EventHandler<SimpleEventArgs> RaiseDragDown;
        public event EventHandler<SimpleEventArgs> RaiseDropItem;

        public DropBox(string name, Vector2 location, Color backColor, Texture2D backTexture)
        {
            this.name = name;
            this.position = location;
            this.backColor = backColor;
            this.backTexture = backTexture;
        }

        public void SetNewItem(string itemName, Texture2D itemTexture)
        {
            this.holding = itemName;
            this.texture = itemTexture;
        }

        public void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch, Vector2 parentPos)
        {
            if (!visible)
                return;

            Vector2 parentOffset = new Vector2(parentPos.X + position.X, parentPos.Y + position.Y);

            //draw border
            if (hasBorder)
            {
                primitiveBatch.Begin(PrimitiveType.LineList);
                primitiveBatch.AddVertex(new Vector2(parentOffset.X - 1, parentOffset.Y - 1), Color.Black);//top left
                primitiveBatch.AddVertex(new Vector2(parentOffset.X + size.X + 1, parentOffset.Y - 1), Color.Black);//top right
                primitiveBatch.AddVertex(new Vector2(parentOffset.X + size.X + 1, parentOffset.Y - 1), Color.Black);//top right
                primitiveBatch.AddVertex(new Vector2(parentOffset.X + size.X + 1, parentOffset.Y + size.Y + 1), Color.Black);//bottom right
                primitiveBatch.AddVertex(new Vector2(parentOffset.X + size.X + 1, parentOffset.Y + size.Y + 1), Color.Black);//bottom right
                primitiveBatch.AddVertex(new Vector2(parentOffset.X - 1, parentOffset.Y + size.Y + 1), Color.Black);//bottom left
                primitiveBatch.AddVertex(new Vector2(parentOffset.X - 1, parentOffset.Y + size.Y + 1), Color.Black);//bottom left
                primitiveBatch.AddVertex(new Vector2(parentOffset.X - 1, parentOffset.Y - 1), Color.Black);//top left
                primitiveBatch.End();
            }

            //draw texture
            spriteBatch.Begin();
            if (hasBorder)
                spriteBatch.Draw(backTexture, new Rectangle((int)parentOffset.X, (int)parentOffset.Y, size.X, size.Y), backColor);
            if(texture != null)
                spriteBatch.Draw(texture, new Rectangle((int)parentOffset.X, (int)parentOffset.Y, size.X, size.Y), Color.White);
            spriteBatch.End();

            
        }

        public void onMouseDown()
        {
            if (!visible)
                return;
            if(canDrag)
                OnRaiseDragDownEvent(new SimpleEventArgs(this.holding));
        }

        public void onMouseOver(Vector2 mousePos)
        {
            if (!visible)
                return;

            if (mousePos.X >= position.X && mousePos.X <= position.X + size.X &&
               mousePos.Y >= position.Y && mousePos.Y <= position.Y + size.Y)
                this.hasBorder = true;
            else if (hasBorder)
                this.hasBorder = false;
        }

        public void click(Vector2 mousePos)
        {
            if (!visible)
                return;

            if (mousePos.X >= position.X && mousePos.X <= position.X + size.X &&
               mousePos.Y >= position.Y && mousePos.Y <= position.Y + size.Y)
                OnRaiseClickEvent(new SimpleEventArgs(this.name));
        }

        public void onMouseUp()
        {
            if (!visible)
                return;
            OnRaiseDropEvent(new SimpleEventArgs(this.holding));
        }

        protected virtual void OnRaiseDropEvent(SimpleEventArgs e)
        {
            EventHandler<SimpleEventArgs> handler = RaiseDropItem;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnRaiseClickEvent(SimpleEventArgs e)
        {
            EventHandler<SimpleEventArgs> handler = RaiseButtonClick;

            if (handler != null)
            {
                e.Message = this.name;
                handler(this, e);
            }
        }

        protected virtual void OnRaiseDragDownEvent(SimpleEventArgs e)
        {
            EventHandler<SimpleEventArgs> handler = RaiseDragDown;

            if (handler != null)
            {
                e.Message = this.holding;
                handler(this, e);
            }
        }
    }

    /// <summary>
    /// textbox input class, activeInputHolder needs this control to realy input
    /// </summary>
    class TextBox
    {
        private string name;
        private bool active = false;
        private bool masked = false;
        private string text = "";
        private Vector2 position;
        private SpriteFont font;
        private Point size = new Point(100, 25);
        private Color backColor;
        private Texture2D backTexture;
        
        public bool Active { get { return active; } set { active = value; } }
        public bool Masked { get { return masked; } set { masked = value; } }
        public string Text { get { return text; } set { text = value; } }
        public string Name { get { return name; } set { name = value; } }
        public Vector2 Position { get { return position; } set { position = value; } }
        public SpriteFont Font { get { return font; } set { font = value; } }
        public Color BackColor { get { return backColor; } set { backColor = value; } }
        public Texture2D BackTexture { get { return backTexture; } set { backTexture = value; } }
        public Point Size { get { return size; } set { size = value; } }

        public TextBox() { }

        public TextBox(string name,Vector2 location,SpriteFont font,Point size,Color backColor,Texture2D backTexture)
        {
            this.name = name;
            this.position = location;
            this.font = font;
            this.size = size;
            this.backColor = backColor;
            this.backTexture = backTexture;
        }

        public void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch, Vector2 parentPos)
        {
            Vector2 parentOffset = new Vector2(parentPos.X + position.X, parentPos.Y + position.Y);
            spriteBatch.Begin();
            spriteBatch.Draw(backTexture, new Rectangle((int)parentOffset.X, (int)parentOffset.Y, size.X, size.Y), backColor);
            Vector2 tmpPos = new Vector2(parentPos.X + position.X + 5,parentPos.Y + position.Y + 3);
            if (!masked)
                spriteBatch.DrawString(font, text + (this.active ? "_" : ""), tmpPos, Color.Black);
            else
            {
                string tmp = "";
                for (int i = 0; i < text.Length; i++)
                    tmp += "*";
                spriteBatch.DrawString(font, tmp + (this.active ? "_" : ""), tmpPos, Color.Black);
            }
            spriteBatch.End();

            primitiveBatch.Begin(PrimitiveType.LineList);
            primitiveBatch.AddVertex(new Vector2(parentOffset.X, parentOffset.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(parentOffset.X + size.X, parentOffset.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(parentOffset.X + size.X, parentOffset.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(parentOffset.X + size.X, parentOffset.Y + size.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(parentOffset.X + size.X, parentOffset.Y + size.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(parentOffset.X, parentOffset.Y + size.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(parentOffset.X, parentOffset.Y + size.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(parentOffset.X, parentOffset.Y), Color.Black);
            primitiveBatch.End();

        }

        public void addText(string input)
        {
            if (input == "bs")
            {
                if (this.Text.Length > 0)
                    this.Text = this.Text.Remove(this.Text.Length - 1, 1);
            }
            else if(this.text.Length < 30)
                this.Text += input;
        }
    }

    /// <summary>
    /// label class, prints text
    /// </summary>
    class Label
    {
        private string name;
        private string text = "";
        private int textBreak = 41;
        private bool visible = true;
        private Vector2 position;
        private SpriteFont font;
        private Color textColor = Color.Black;

        public string Name { get { return name; } set { name = value; } }
        public string Text { get { return text; } set { text = value; } }
        public bool Visible { get { return visible; } set { visible = value; } }
        public int TextBreak { get { return textBreak; } set { textBreak = value; } }
        public Vector2 Position { get { return position; } set { position = value; } }
        public SpriteFont Font { get { return font; } set { font = value; } }
        
        public Label() { }

        public Label(string name, string text,Color textColor, Vector2 location, SpriteFont font)
        {
            this.name = name;
            this.text = text;
            this.textColor = textColor;
            this.position = location;
            this.font = font;
        }

        public void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch, Vector2 parentPos)
        {
            if (!visible)
                return;

            Vector2 tmpPos = new Vector2(parentPos.X + position.X, parentPos.Y + position.Y);
            string wrapped = this.text;
            int c = 0;
            for (int i = textBreak; i < text.Length; i += textBreak, c++)
            {
                int newline = wrapped.Substring(i - textBreak, textBreak).LastIndexOf("\n");
                if (newline > 0)
                {
                    i -= textBreak - newline;
                    c--;
                    continue;
                }
                wrapped = wrapped.Insert(i + c, wrapped.Substring(i + c - 1, 1) != " " && wrapped.Substring(i + c, 1) != " " ? "-\n" : "\n");
            }
            spriteBatch.Begin();
            spriteBatch.DrawString(font, wrapped, tmpPos, textColor);
            spriteBatch.End();
        }
    }

    /// <summary>
    /// event info 
    /// </summary>
    public class SimpleEventArgs : EventArgs
    {
        public SimpleEventArgs(string s)
        {
            message = s;
        }
        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }

    /// <summary>
    /// custom draw
    /// </summary>
    public class CustomDrawEventArgs : EventArgs
    {
        private string who;
        private Vector2 position;

        public string Who { get { return who; } set { who = value; } }
        public Vector2 Position { get { return position; } set { position = value; } }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="s">who is calling</param>
        /// <param name="pos">where to draw</param>
        public CustomDrawEventArgs(string who, Vector2 pos)
        {
            this.who = who;
            this.position = pos;
        }
    }

    /// <summary>
    /// button class
    /// </summary>
    class Button
    {
        private string name;
        private string text = "";
        private bool mouseDown = false;
        private bool visible = true;
        private Vector2 position;
        private SpriteFont font;
        private Color textColor = Color.Black;
        private Color backColor;
        private Texture2D backTexture;
        private Point size;
        

        public string Name { get { return name; } set { name = value; } }
        public string Text { get { return text; } set { text = value; } }
        public bool Visible { get { return visible; } set { visible = value; } }
        public Point Size { get { return size; } set { size = value; } }
        public Vector2 Position { get { return position; } set { position = value; } }
        public SpriteFont Font { get { return font; } set { font = value; } }
        public Color BackColor { get { return backColor; } set { backColor = value; } }
        public Texture2D BackTexture { get { return backTexture; } set { backTexture = value; } }

        public Button() { }

        public Button(string name, string text, Color textColor, Vector2 location, Point size, SpriteFont font,
                      Color backColor,Texture2D backTexture)
        {
            this.name = name;
            this.text = text;
            this.textColor = textColor;
            this.position = location;
            this.font = font;
            this.backColor = backColor;
            this.backTexture = backTexture;
            this.size = size;
        }

        public event EventHandler<SimpleEventArgs> RaiseButtonClick;

        protected virtual void OnRaiseClickEvent(SimpleEventArgs e)
        {
            EventHandler<SimpleEventArgs> handler = RaiseButtonClick;

            if (handler != null)
            {
                e.Message = this.name;
                handler(this, e);
            }
        }

        public void click()
        {
            if (!visible)
                return;

            OnRaiseClickEvent(new SimpleEventArgs(this.name));
        }

        public void onMouseDown()
        {
            if (!visible)
                return;

            if (!mouseDown)
            {
                Color tmp = this.backColor;
                this.backColor = this.textColor;
                this.textColor = tmp;
            }
            mouseDown = true;
        }

        public void onMouseUp()
        {
            if (!visible)
                return;

            if (mouseDown)
            {
                Color tmp = this.backColor;
                this.backColor = this.textColor;
                this.textColor = tmp;
            }
            mouseDown = false;
        }

        public void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch, Vector2 parentPos)
        {
            if (!visible)
                return;

            Vector2 parentOffset = new Vector2(parentPos.X + position.X, parentPos.Y + position.Y);
            Vector2 tmpPos = new Vector2(parentOffset.X + size.X / 2 - text.Length * 7 / 2, parentOffset.Y + 3);
            spriteBatch.Begin();
            spriteBatch.Draw(backTexture, new Rectangle((int)parentOffset.X, (int)parentOffset.Y, size.X, size.Y), backColor);
            spriteBatch.DrawString(font, text, tmpPos, textColor);
            spriteBatch.End();

            primitiveBatch.Begin(PrimitiveType.LineList);
            //frame
            primitiveBatch.AddVertex(new Vector2(parentOffset.X, parentOffset.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(parentOffset.X + size.X, parentOffset.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(parentOffset.X + size.X, parentOffset.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(parentOffset.X + size.X, parentOffset.Y + size.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(parentOffset.X + size.X, parentOffset.Y + size.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(parentOffset.X, parentOffset.Y + size.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(parentOffset.X, parentOffset.Y + size.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(parentOffset.X, parentOffset.Y), Color.Black);

            //second frame
            primitiveBatch.AddVertex(new Vector2(parentOffset.X + 2, parentOffset.Y + 2), Color.Black);
            primitiveBatch.AddVertex(new Vector2(parentOffset.X + size.X - 2, parentOffset.Y + 2), Color.Black);
            primitiveBatch.AddVertex(new Vector2(parentOffset.X + size.X - 2, parentOffset.Y + 2), Color.Black);
            primitiveBatch.AddVertex(new Vector2(parentOffset.X + size.X - 2, parentOffset.Y + size.Y - 2), Color.Black);
            primitiveBatch.AddVertex(new Vector2(parentOffset.X + size.X - 2, parentOffset.Y + size.Y - 2), Color.Black);
            primitiveBatch.AddVertex(new Vector2(parentOffset.X + 2, parentOffset.Y + size.Y - 2), Color.Black);
            primitiveBatch.AddVertex(new Vector2(parentOffset.X + 2, parentOffset.Y + size.Y - 2), Color.Black);
            primitiveBatch.AddVertex(new Vector2(parentOffset.X + 2, parentOffset.Y + 2), Color.Black);

            primitiveBatch.End();
        }
    }

}
