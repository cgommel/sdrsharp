using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel.Design;

namespace SDRSharp.CollapsiblePanel
{
    /// <summary>
    /// Enumeration for defining the options for panel state
    /// </summary>
    public enum PanelStateOptions
    {
        Collapsed,
        Expanded
    }

    /// <summary>
    /// Main class for collapsible panel user control
    /// </summary>
    [DesignTimeVisible(true)]
    [Category("Containers")]
    [Description("Visual Studio like Collapsible Panel")]
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))] 
    public partial class CollapsiblePanel : UserControl
    {
        #region Class members
        #region Variables
        /// <summary>
        /// Variable for setting the user control height when control is expanded
        /// </summary>
        int _expandedHeight;
        /// <summary>
        /// Variable for setting the current user control state
        /// </summary>
        PanelStateOptions _panelState = PanelStateOptions.Expanded;
        /// <summary>
        /// Variable for determining if the user control is currently collapsed
        /// </summary>
        bool _isCollapsed = false;
        /// <summary>
        /// Determines if this user control is going to match its parent width
        /// </summary>
        bool _fitToParent = false;
        /// <summary>
        /// Panel to be located beneath this panel
        /// </summary>
        CollapsiblePanel _nextPanel;
        #endregion

        #region Delegates and events
        /// <summary>
        /// Delegate and event for informing the parent control that this user control state has changed
        /// </summary>
        public delegate void DelegateStateChanged();
        [Category("Collapsible Panel")]
        public event DelegateStateChanged StateChanged;
        #endregion

        #region Properties
        
        /// <summary>
        /// Gets or sets the value for user control height when it is expanded
        /// </summary>
        [Description("Gets or sets the value for user control height when it is expanded")]
        [DisplayName("Expanded Height")]
        [Category("Collapsible Panel")]
        [DefaultValueAttribute(0)]
        public int ExpandedHeight
        {
            get { return _expandedHeight; }
            set
            {
                if (value > 0)
                {
                    if (this.DesignMode)
                    {
                        if (_panelState == PanelStateOptions.Expanded)
                        {   //Setting Expanded Height is only allowed when user control is expanded
                            this.SetBounds(this.Location.X,
                                           this.Location.Y,
                                           this.Size.Width,
                                           titlePanel.Height + value);
                        }
                    }
                    else
                    {
                        _expandedHeight = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets panel title"
        /// </summary>
        [Description("Gets or sets panel title")]
        [DisplayName("Panel Title")]
        [Category("Collapsible Panel")]
        public string PanelTitle
        {
            get { return lblPanelTitle.Text; }
            set { lblPanelTitle.Text = value; }
        }

        /// <summary>
        /// Gets or sets current panel state
        /// </summary>
        [DefaultValue(typeof(PanelStateOptions),"Expanded")]
        [Description("Gets or sets current panel state")]
        [DisplayName("Panel State")]
        [Category("Collapsible Panel")]
        public PanelStateOptions PanelState
        {
            get { return _panelState; }
            set { 
                    _panelState = value;
                    _isCollapsed = (_panelState != PanelStateOptions.Collapsed);
                    ToggleState(null,null);
                 }
             }

        /// <summary>
             /// If True, fits the panel to match the parent width
        /// </summary>
        [Category("Collapsible Panel")]
        [DesignOnly(true)]
        [DefaultValue(false)]
        [Description("If True, fits the panel to match the parent width")]
        public bool FitToParent
        {
            get { return _fitToParent; }
            set { 
                _fitToParent = value;
                ////if (this.DesignMode)
                ////{
                    if (_fitToParent)
                    {
                        if (this.Parent != null)
                        {
                            this.Location = new Point(0, this.Location.Y);
                            this.Size = new Size(this.Parent.Size.Width, this.Size.Height);
                            this.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                        }
                    }
                    else
                    {
                        this.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                    }
                ////}
            }
        }

        /// <summary>
        /// Gets or sets the panel to be located beneath this panel
        /// </summary>
        [Category("Collapsible Panel")]
        [Description("Gets or sets the panel to be located beneath this panel")]
        public CollapsiblePanel NextPanel
        {
            get { return _nextPanel; }
            set { _nextPanel = value;
            MoveNextPanel();
        }
        }
        #endregion

        #endregion

        #region Class constructor
             public CollapsiblePanel()
        {
            InitializeComponent();

            ////if (!this.DesignMode)
            ////{
            ////    _expandedHeight = contentPanel.Height;
                    
            ////}
            ////else
            ////{
            ////    contentPanel.Height = this.Height - titlePanel.Height;
            ////}

            this.Load += new EventHandler(CollapsiblePanel_Load);
            this.SizeChanged += new EventHandler(CollapsiblePanel_SizeChanged);
            this.LocationChanged += new EventHandler(CollapsiblePanel_LocationChanged);
                 
        }

        
        #endregion

        #region Methods for handling user control events
        
        void CollapsiblePanel_Load(object sender, EventArgs e)
        {
            ///Determining and setting the initial toggling image
            _isCollapsed = (_panelState == PanelStateOptions.Collapsed);
            
            if (_isCollapsed)
            {
                togglingImage.Image = global::SDRSharp.CollapsiblePanel.Properties.Resources.ExpandedIcon;
            }
            else
            {
                togglingImage.Image = global::SDRSharp.CollapsiblePanel.Properties.Resources.CollapsedIcon;
            }
        }
        
        /// <summary>
        /// We use this event to recalculate the Expanded Height when the user resizes the user control at Design mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CollapsiblePanel_SizeChanged(object sender, EventArgs e)
        {
                if (this.DesignMode)
                {
                    if (_panelState == PanelStateOptions.Expanded)
                    {
                        _expandedHeight = this.Height - titlePanel.Height;
                    }
                    else
                    {
                        ///Final user can only resize the user control when it is expanded
                        this.SetBounds(this.Location.X,
                                       this.Location.Y,
                                       this.Size.Width,
                                       titlePanel.Height);
                    }

                    if (this.Parent != null)
                    {
                        if (this.Parent.Size.Width != this.Size.Width)
                        {
                            FitToParent = false;
                        }
                    }
                }

                MoveNextPanel();
        }

        /// <summary>
        /// We use this event in order to move the next panel down when this panel located is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CollapsiblePanel_LocationChanged(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                if (this.Location.X > 0)
                {
                    FitToParent = false;
                }
            }

            MoveNextPanel();
        }

        
        #endregion

        #region Class Methods
        /// <summary>
        /// Changes the current state from Collapsed to Expanded or viceversa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleState(object sender, EventArgs e)
        {
            if (_isCollapsed)
            {
                ///Current state is Collapsed.  Expand the user control
                this.SetBounds(this.Location.X,
                                       this.Location.Y,
                                       this.Size.Width,
                                       titlePanel.Height + _expandedHeight);
            }
            else
            {
                ///Current state is Expanded.  Collapse the user control
                this.SetBounds(this.Location.X,
                                       this.Location.Y,
                                       this.Size.Width,
                                       titlePanel.Height);
            }

            _isCollapsed = !_isCollapsed;

            ///Setting content control current state and toggling image
            if (_isCollapsed)
            {
                _panelState = PanelStateOptions.Collapsed;
                togglingImage.Image = global::SDRSharp.CollapsiblePanel.Properties.Resources.ExpandedIcon;
            }
            else
            {
                _panelState = PanelStateOptions.Expanded;
                togglingImage.Image = global::SDRSharp.CollapsiblePanel.Properties.Resources.CollapsedIcon;
            }

            if (!this.DesignMode)
            {
                if (StateChanged != null)
                {
                    ///Fire the event to inform the parent control that the state for the user control has changed
                    StateChanged();
                }
            }
        }

        /// <summary>
        /// Moves the next panel down (when user control state is changed or the control is relocated)
        /// </summary>
        private void MoveNextPanel()
        {
                if (_nextPanel != null)
                {
                    _nextPanel.Location = new Point(_nextPanel.Location.X,
                                                    this.Location.Y + this.Size.Height);
                }
        }
        #endregion
    }
}