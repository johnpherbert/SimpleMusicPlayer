   M S B A M L   `   `   `  ����L  HSimpleMusicPlayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:SimpleMusicPlayerSimpleMusicPlayer  5   	   U/clr-namespace:SimpleMusicPlayer.Models.FileTree!SimpleMusicPlayer.Models.FileTree  5   	   c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity 5	   	   "    SimpleMusicPlayer.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:SimpleMusicPlayer     5   	   CDirTree/clr-namespace:SimpleMusicPlayer.Models.FileTree     5   	   ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5	   	   -   5          �Name$  MusicWindow��5   	    � Title$ 
MainWindow��5   	   $	��750��6   $	��525��6)    � DataContext 5
   	   �� *Main+   Locator �� Source# [   � 	Resources 5       �� DataType5   /SimpleMusicPlayer.Models.FileTree.DirectoryItem �� ItemsSource �� Path � ToolTip0   *SimpleMusicPlayer.Models.FileTree.FileItem��%  (��       5   #   *, +)(�� �     5   #   *, +)�� 5   
   "  6#    5   #   �� $ Items�.R�5      �� ��6   �� $
 Name� 63   �� $
 Path�5      �� 5   
   "  6#   .R�5      �� ��6   �� $
 Name� 63   �� $
 Name�5      5      .��5      � 	 � ColumnDefinitions	 5   
   �� 5      $	��.4* �6   �� 5      $	��.8* �6   
 � RowDefinitions
 5   
   �� 5      $	��.1* �6   �� 5      $	��.9* �6   .+�5!   
   o� $
��Auto��6   $��20��6   .@�5"      m� $��_File��6   .@�5#      m� $��	_Settings��6   $
��Left=�6.   $	��140��6I    m� Command 6U   �� *OpenSettingsCommand+5%      m� 5&      $��_Help��6   .@�5'      m� $��_About��6   $
��Left=�6+   $	��140��6F   5(      5)      R� 5+   
   -   5         R�Name$ filetree��5+      $��0��6%   $��1��65   ��6B   �� $ Items�� 5-   
   $��1��6   $��1��6   
 5.      �� 5/      $��50 �6    �� 50      $��30 �6    �� 51      $��* �6    .+�54      ��  �� Orientation$ Vertical=�6   $��0��60   .+�55      �� $ 
Horizontal=�6   $��Center=�66   $��Center=�6S   .+�56      �� $��30��6   $��30��6)   ��64   �� *PlaySongCommand+��6X   ��  �� ElementName$ currentplaylistview��$ SelectedItem�.��6�   Play6�   �� 57      $��30��6   $��30��6)   ��64   �� *PauseSongCommand+.��6Y   Pause6`   �� 58      $��30��6   $��30��6)   ��64   �� *StopSongCommand+.��6X   Stop6^   59      �� 5:      $��Center=�6   $��Center=�66   $	��300��6Q    � Minimum$ 0Q�6]    � Maximum$	 100Q�6i    � Value 6w   �� $ MusicPlayer.Volume�5;      �� 5=      $��1��6   $ 
Horizontal=�6&   $��Center=�6?   $��Center=�6\   .+�5>      �� $��50��6   ��6$   �� *AddSongCommand+��6G   �� $ filetree��$ SelectedItem�.��6�   +6�   �� 5?      $��50��6   ��6$   �� *RemoveSongCommand+��6J   �� $ currentplaylistview��.��6�   -6�   5@      �� 5A      $��2��6   .��5B      �� -   5         ��Name$ currentplaylistview��5B      ��69   �� *MusicPlayer.CurrentPlaylist+ �� View 5C      �� 5D      .P�5E      �� $
��Name��6-    �� Width$	 200��6;    �� DisplayMemberBinding 6G   �� $
 Name�5F      5H      5I      5J      5K      .  (System.Windows.Interactivity.Interaction   Triggers 5L      /  )System.Windows.Interactivity.EventTrigger  5M   
      	EventName$ Closing��6   .  (System.Windows.Interactivity.TriggerBase   Actions. 5N      6  0System.Windows.Interactivity.InvokeCommandAction     Command 6$   �� *ExitCommand+5O      5Q      