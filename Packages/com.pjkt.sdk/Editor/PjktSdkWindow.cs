using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PJKT.SDK2.NET;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PJKT.SDK2
{
    public class PjktSdkWindow : EditorWindow
    {
        [MenuItem("PJKT SDK/SDK2")]
        public static void ShowWindow()
        {
            var window = GetWindow<PjktSdkWindow>();
            window.titleContent = new GUIContent("PJKT SDK2");
            window.minSize = new Vector2(256, 512);
        }
        
        private VisualElement _sdkPageView;
        private ScrollView _tabsScrollView;
        private Notification _notification;

        SdkTab _selectedTab;
        private async void OnEnable()
        {
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.pjkt.sdk/Editor/Visual Elements/PjktSdk2xml.uxml");
            var clone = asset.Instantiate();
            clone.style.flexGrow = 1;
            rootVisualElement.Add(clone);
            
            _sdkPageView = rootVisualElement.Q<VisualElement>("Content");
            _tabsScrollView = rootVisualElement.Q<ScrollView>("TabList");
            
            VisualElement notificationContainer = rootVisualElement.Q<VisualElement>("Notification_Container");
            _notification = new Notification();
            notificationContainer.Add(_notification);
            _notification.style.translate = new Translate(0, 128);

            Authentication.OnLoginStatusChanged += OnLoginChanged;
            
            CreateTabs();
            BoothValidator.GetBoothsInScene();
            PjktEventManager.GetEvents();

            await Authentication.TryResumeSession();
            
            if (!Authentication.IsLoggedIn) ShowLogin();
        }

        private void OnDisable()
        {
            Authentication.OnLoginStatusChanged -= OnLoginChanged;
        }

        //feel free to reorder these
        private void CreateTabs()
        {
            //booth info Tab
            var homeTab = new SdkTab(PjktGraphics.GetGraphic("Booths"), PjktGraphics.GraphicColors["Booths"], SDKPageType.Booths);
            Button homeTabButton = homeTab.Q<Button>("TabButton");
            homeTabButton.tooltip = "Booth Info";
            homeTabButton.RegisterCallback<ClickEvent>(OnTabClick);
            _tabsScrollView.Add(homeTab);
            
            //Event Tab
            var eventsTab = new SdkTab(PjktGraphics.GetGraphic("EventTicket"), PjktGraphics.GraphicColors["EventTicket"], SDKPageType.Events);
            Button eventsTabButton = eventsTab.Q<Button>("TabButton");
            eventsTabButton.tooltip = "Events";
            eventsTabButton.RegisterCallback<ClickEvent>(OnTabClick);
            _tabsScrollView.Add(eventsTab);
            
            //Meshes Tab
            var meshTab = new SdkTab(PjktGraphics.GetGraphic("Mesh"), PjktGraphics.GraphicColors["Mesh"], SDKPageType.Meshes);
            Button meshTabButton = meshTab.Q<Button>("TabButton");
            meshTabButton.tooltip = "Meshes";
            meshTabButton.RegisterCallback<ClickEvent>(OnTabClick);
            _tabsScrollView.Add(meshTab);
            
            //Materials Tab
            var materialTab = new SdkTab(PjktGraphics.GetGraphic("Material"), PjktGraphics.GraphicColors["Material"], SDKPageType.Materials);
            Button materialTabButton = materialTab.Q<Button>("TabButton");
            materialTabButton.tooltip = "Materials";
            materialTabButton.RegisterCallback<ClickEvent>(OnTabClick);
            _tabsScrollView.Add(materialTab);
            
            //Textures Tab
            var textureTab = new SdkTab(PjktGraphics.GetGraphic("Texture"), PjktGraphics.GraphicColors["Texture"], SDKPageType.Textures);
            Button textureTabButton = textureTab.Q<Button>("TabButton");
            textureTabButton.tooltip = "Textures";
            textureTabButton.RegisterCallback<ClickEvent>(OnTabClick);
            _tabsScrollView.Add(textureTab);
            
            //Animations Tab
            var animationTab = new SdkTab(PjktGraphics.GetGraphic("Animation"), PjktGraphics.GraphicColors["Animation"], SDKPageType.Animations);
            Button animationTabButton = animationTab.Q<Button>("TabButton");
            animationTabButton.tooltip = "Animations";
            animationTabButton.RegisterCallback<ClickEvent>(OnTabClick);
            _tabsScrollView.Add(animationTab);
            
            //Text Tab
            var textTab = new SdkTab(PjktGraphics.GetGraphic("Text"), PjktGraphics.GraphicColors["Text"], SDKPageType.Text);
            Button textTabButton = textTab.Q<Button>("TabButton");
            textTabButton.tooltip = "Text";
            textTabButton.RegisterCallback<ClickEvent>(OnTabClick);
            _tabsScrollView.Add(textTab);
            
            //Particles Tab
            var particlesTab = new SdkTab(PjktGraphics.GetGraphic("Particles"), PjktGraphics.GraphicColors["Particles"], SDKPageType.Particles);
            Button particlesTabButton = particlesTab.Q<Button>("TabButton");
            particlesTabButton.tooltip = "Particles";
            particlesTabButton.RegisterCallback<ClickEvent>(OnTabClick);
            _tabsScrollView.Add(particlesTab);
            
            //SDK Components Tab
            var sdkCompTab = new SdkTab(PjktGraphics.GetGraphic("VRC"), PjktGraphics.GraphicColors["Avatar"], SDKPageType.VRCComponents);
            Button sdkCompTabButton = sdkCompTab.Q<Button>("TabButton");
            sdkCompTabButton.tooltip = "VRC Components";
            sdkCompTabButton.RegisterCallback<ClickEvent>(OnTabClick);
            _tabsScrollView.Add(sdkCompTab);
            
            //Settings Tab
            var settingsTab = new SdkTab(PjktGraphics.GetGraphic("Settings"), PjktGraphics.GraphicColors["Settings"], SDKPageType.Settings);
            Button settingsTabButton = settingsTab.Q<Button>("TabButton");
            settingsTabButton.tooltip = "Settings";
            settingsTabButton.RegisterCallback<ClickEvent>(OnTabClick);
            _tabsScrollView.Add(settingsTab);
        }

        internal async void OnLoginChanged(object sender, EventArgs e)
        {
            //Debug.Log("user logged in = " + Authentication.IsLoggedIn);
            if (Authentication.IsLoggedIn)
            {
                //Debug.Log("logged in");
                await HideLogin();
                await HideRegister();
                ShowContent();
                ShowSideBar();

                Notify("Login successful, welcome!", BoothErrorType.Info);
            }
            else
            {
                await HideSideBar();
                HideContent();
                ShowLogin();
                
                Notify("Logged out successfully", BoothErrorType.Info);
            }
        }

        internal async Task ShowRegister()
        {
            await HideLogin();
            VisualElement window = rootVisualElement.Q<VisualElement>("window");
            RegisterForm registerForm = new RegisterForm();
            window.Add(registerForm);
            
            //Debug.Log("showed register");
        }

        private async Task HideRegister()
        {
            VisualElement window = rootVisualElement.Q<VisualElement>("window");
            RegisterForm registerForm = window.Q<RegisterForm>();
            
            if (registerForm == null) return;
            
            registerForm.style.transitionDuration = new List<TimeValue> {new TimeValue(300, TimeUnit.Millisecond)};
            registerForm.style.translate = new Translate(0, -230);
            
            Notify("Login successful, welcome!", BoothErrorType.Info);
            
            //wait for the transition to finish
            await Task.Delay(300);

            window.Remove(registerForm);
            
            //Debug.Log("hid register");
        }

        private void ShowLogin()
        {
            VisualElement window = rootVisualElement.Q<VisualElement>("window");
            
            LoginForm existingform = window.Q<LoginForm>();
            if (existingform != null) return;
            
            LoginForm loginForm = new LoginForm();
            window.Add(loginForm);
            
            //Debug.Log("showed login");
        }
        private async Task HideLogin()
        {
            //Debug.Log("hiding login");
            VisualElement window = rootVisualElement.Q<VisualElement>("window");
            LoginForm loginForm = window.Q<LoginForm>();
            
            if (loginForm == null) return;
            
            loginForm.style.transitionDuration = new List<TimeValue> {new TimeValue(300, TimeUnit.Millisecond)};
            loginForm.style.translate = new Translate(0, -230);
            
            //Debug.Log("about to await");
            //wait for the transition to finish
            await Task.Delay(300);

            window.Remove(loginForm);
            //Debug.Log("hid login");
        }
        
        private void ShowSideBar()
        {
            _tabsScrollView.style.translate = new Translate(0, 0);
            _tabsScrollView.style.transitionDelay = new List<TimeValue> {0, 0};
            _tabsScrollView.style.display = DisplayStyle.Flex;
            
            //Debug.Log("showed sidebar");
        }
        
        private async Task HideSideBar()
        {
            _tabsScrollView.style.translate = new Translate(-64, 0);
            _tabsScrollView.style.transitionDelay = new List<TimeValue> {0, new TimeValue(300, TimeUnit.Millisecond)};
            _tabsScrollView.style.display = DisplayStyle.None;
            
            await Task.Delay(300);
            //Debug.Log("hid sidebar");
        }
        
        private void ShowContent()
        {
            _sdkPageView.style.display = DisplayStyle.Flex;

            //Debug.Log("showed content");
        }
        
        private void HideContent()
        {
            if (_selectedTab != null) _selectedTab.SelectTab(false);
            _sdkPageView.Clear();
            _sdkPageView.style.display = DisplayStyle.None;
            
            //Debug.Log("hid content");
        }

        //this whole thing is dumb
        Queue<string> pendingNotifications = new Queue<string>();
        Queue<BoothErrorType> pendingNotificationTypes = new Queue<BoothErrorType>();
        private Task notificationTask = null;
        
        public static void Notify(string notification, BoothErrorType type = BoothErrorType.Info)
        {
            PjktSdkWindow window = GetWindow<PjktSdkWindow>();
            window._Notify(notification, type);
        }
        
        private void _Notify(string notification, BoothErrorType type = BoothErrorType.Info)
        {
            if (pendingNotifications.Contains(notification)) return;
            pendingNotifications.Enqueue(notification);
            pendingNotificationTypes.Enqueue(type);

            if (notificationTask != null) return;
            
            notificationTask = DisplayNotifications();
        }

        private async Task DisplayNotifications()
        {
            _notification.style.display = DisplayStyle.Flex;
            
            //go through all the notifications in the queue
            while (pendingNotifications.Count > 0)
            {
                string text = pendingNotifications.Peek();
                BoothErrorType type = pendingNotificationTypes.Peek();
                
                //display the notification
                _notification.SetNotification(text, type);
                
                //throw it in the editor log
                switch (type)
                {
                    case BoothErrorType.Info:
                        Debug.Log($"<color=#FFBB00><b>PJKT SDK:</b></color> {text}");
                        break;
                    case BoothErrorType.Warning:
                        Debug.LogWarning($"<color=#FFBB00><b>PJKT SDK:</b></color> {text}");
                        break;
                    case BoothErrorType.Error:
                        Debug.LogError($"<color=#FFBB00><b>PJKT SDK:</b></color> {text}");
                        break;
                    default:
                        Debug.Log($"<color=#FFBB00><b>PJKT SDK:</b></color> {text}");
                        break; 
                }

                //translate it up
                _notification.ShowNotification();
                await Task.Delay(3000);
                
                //translate it down
                _notification.HideNotification();
                await Task.Delay(300);
                
                //remove it from the queue
                pendingNotifications.Dequeue();
                pendingNotificationTypes.Dequeue();
            }
            notificationTask = null;
            _notification.style.display = DisplayStyle.None;
        }

        private async void OnTabClick(ClickEvent clk)
        {
            VisualElement button = clk.currentTarget as VisualElement;
            SdkTab tab = button.parent as SdkTab;
            if (tab == null) return;
            if (_selectedTab == tab) return;
            
            if (_selectedTab != null) _selectedTab.SelectTab(false);
            
            //swipe away the current page
            _sdkPageView.style.transitionDuration = new List<TimeValue> {new TimeValue(300, TimeUnit.Millisecond)};
            _sdkPageView.style.translate = new Translate(Screen.width, 0);
            await Task.Delay(300);
            _sdkPageView.Clear();
            
            _selectedTab = tab;
            _selectedTab.SelectTab(true);
            
            //teleport page
            _sdkPageView.style.transitionDuration = new List<TimeValue> {new TimeValue(0, TimeUnit.Millisecond)};
            _sdkPageView.style.translate = new Translate(-Screen.width, 0);
            
            
            //show the new page
            SDKPage newPage = GetSDKPage(tab.pageType);
            if (newPage == null) return;
            _sdkPageView.Add(newPage);
            newPage.OnTabEnable();
            _sdkPageView.style.transitionDuration = new List<TimeValue> {new TimeValue(300, TimeUnit.Millisecond)};
            _sdkPageView.style.translate = new Translate(0, 0);
        }

        public void RefreshPage()
        {
            if (_selectedTab == null) return;
            _selectedTab.SelectTab(false);
            _sdkPageView.Clear();
            _selectedTab.SelectTab(true);
            
            SDKPage newPage = GetSDKPage(_selectedTab.pageType);
            if (newPage == null) return;
            _sdkPageView.Add(newPage);
            newPage.OnTabEnable();
        }

        private SDKPage GetSDKPage(SDKPageType type)
        {
            SDKPage page = null;
            switch (type)
            {
                case SDKPageType.Booths:
                    page = new BoothPage();
                    break;
                case SDKPageType.Events:
                    page = new EventsPage();
                    break;
                case SDKPageType.Meshes:
                    page = new MeshesPage();
                    break;
                case SDKPageType.Materials:
                    page = new MaterialsPage();
                    break;
                case SDKPageType.Textures:
                    page = new TexturesPage();
                    break;
                case SDKPageType.Animations:
                    page = new AnimationsPage();
                    break;
                case SDKPageType.Text:
                    page = new TextsPage();
                    break;
                case SDKPageType.Particles:
                    page = new ParticlesPage();
                    break;
                case SDKPageType.VRCComponents:
                    page = new VrcCompsPage();
                    break;
                case SDKPageType.Settings:
                    page = new SettingsPage();
                    break;
            }

            return page;
        }
    }   
    
    public enum SDKPageType
    {
        Booths,
        Events,
        Meshes,
        Materials,
        Textures,
        Animations,
        Text,
        Particles,
        VRCComponents,
        Settings,
    }
}