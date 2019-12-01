﻿using DevPrompt.Api;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisualStudioMenu.Plugin.Utility;

namespace VisualStudioMenu.Plugin
{
    /// <summary>
    /// Adds custom menu items to the main window
    /// </summary>
    [Export(typeof(ICommandProvider))]
    public class VisualStudioMenuProvider : ICommandProvider
    {
        private readonly IApp app;

        [ImportingConstructor]
        public VisualStudioMenuProvider(IApp app)
        {
            this.app = app;
        }

        IEnumerable<KeyBinding> ICommandProvider.GetKeyBindings(IWindow window) => Enumerable.Empty<KeyBinding>();

        IEnumerable<FrameworkElement> ICommandProvider.GetMenuItems(MenuType menu, IWindow window)
        {
            if (menu == MenuType.MenuBar)
            {
                MenuItem rootItem = new MenuItem()
                {
                    Header = Resources.Menu_Name,
                };

                rootItem.SubmenuOpened += this.OnMenuOpened;

                rootItem.Items.Add(new MenuItem()
                {
                    Header = Resources.Menu_VsUpdating,
                    IsEnabled = false,
                });

                rootItem.Items.Add(new Separator()
                {
                    Tag = "[End]",
                });

                rootItem.Items.Add(new MenuItem()
                {
                    Header = this.app.IsMicrosoftDomain ? Resources.Menu_VsInstallDogfood : Resources.Menu_VsInstallFromWeb,
                    Command = this.OnlineInstallerCommand,
                });

                if (this.app.IsMicrosoftDomain)
                {
                    rootItem.Items.Add(new MenuItem()
                    {
                        Header = Resources.Menu_VsInstallBranch,
                        Command = this.BranchInstallerCommand(window),
                    });
                }

                rootItem.Items.Add(new MenuItem()
                {
                    Header = Resources.Menu_VsInstaller,
                    Command = this.LocalInstallerCommand,
                });

                yield return rootItem;
            }
        }

        private async void OnMenuOpened(object sender, RoutedEventArgs args)
        {
            MenuItem menu = (MenuItem)sender;
            IEnumerable<IVisualStudioInstance> instances = await this.app.VisualStudioSetup.GetInstancesAsync();

            VisualStudioMenuProvider.UpdateMenu(menu, instances.ToArray(), (IVisualStudioInstance instance) =>
            {
                return new MenuItem()
                {
                    Header = instance.DisplayName,
                    Command = new DelegateCommand((object arg) => this.Start((IVisualStudioInstance)arg)),
                    CommandParameter = instance,
                };
            });
        }

        private void Start(IVisualStudioInstance instance)
        {
            this.app.Telemetry.TrackEvent("Start.VS");
            this.app.RunExternalProcess(instance.ProductPath);
        }

        private ICommand LocalInstallerCommand => new DelegateCommand(() =>
        {
            this.app.Telemetry.TrackEvent("Command.VisualStudioInstaller");
            this.app.RunExternalProcess(this.app.VisualStudioSetup.LocalInstallerPath);
        });

        private ICommand OnlineInstallerCommand => new DelegateCommand(() =>
        {
            this.app.Telemetry.TrackEvent("Command.VisualStudioOnlineInstaller");
            this.app.RunExternalProcess(this.app.VisualStudioSetup.OnlineInstallerUrl);
        });

        private ICommand BranchInstallerCommand(IWindow window) => new DelegateCommand(() =>
        {
            this.app.Telemetry.TrackEvent("Command.VisualStudioBranchInstaller");

            InstallBranchDialog dialog = new InstallBranchDialog()
            {
                Owner = window.Window,
            };

            if (dialog.ShowDialog() == true)
            {
                this.app.RunExternalProcess(dialog.ViewModel.Hyperlink);
            }
        });

        /// <summary>
        /// Replaces MenuItems up until the first separator with dynamic MenuItems.
        /// The dynamic MenuItems are generated by the createMenuItem Func
        /// </summary>
        private static void UpdateMenu<T>(MenuItem menu, IList<T> dynamicItems, Func<T, Control> createMenuItem)
        {
            for (int i = 0; i < dynamicItems.Count; i++)
            {
                FrameworkElement item = (FrameworkElement)menu.Items[i];

                if (item.Tag is string str && str == "[End]")
                {
                    // Reached the end separator
                    menu.Items.Insert(i, createMenuItem(dynamicItems[i]));
                }
                else if (!(item.Tag is T itemTag) || !EqualityComparer<T>.Default.Equals(itemTag, dynamicItems[i]))
                {
                    FrameworkElement elem = createMenuItem(dynamicItems[i]);
                    elem.Tag = dynamicItems[i];
                    menu.Items[i] = elem;
                }
            }

            // Delete old extra items
            for (int i = dynamicItems.Count; i < menu.Items.Count; i++)
            {
                FrameworkElement item = (FrameworkElement)menu.Items[i];

                if (item.Tag is string str && str == "[End]")
                {
                    item.Visibility = (dynamicItems.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
                    break;
                }
                else
                {
                    menu.Items.RemoveAt(i--);
                }
            }
        }
    }
}