   M S B A M L   `   `   `  ���� H  DStartMenuWin8, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:StartMenuWin8StartMenuWin8  R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R8clr-namespace:System.ComponentModel;assembly=WindowsBaseSystem.ComponentModel     StartMenuWin8.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -localclr-namespace:StartMenuWin8     Hscm8clr-namespace:System.ComponentModel;assembly=WindowsBase     -     �Name$  Startup_Menu�� � Title$ Start��$	��360��$	��535��$��Bottom=�$
��Left=� � Left$ 0��$
��Auto=� � ShowInTaskbar .  � 
ResizeMode$ NoResize=�#��� �� � WindowStyle$
 None=� � 	Resources O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  '  #invisibleButtonTreeViewItemTemplate �� 
TargetType � ColumnDefinitions	 � RowDefinitions
 d� 	IsChecked �� 	ClickMode �� Padding J� SnapsToDevicePixels �� HeaderStringFormat �� ContentStringFormat �� Triggers P� Property �� 
TargetName �� Property �� Value �� Member P� Value  TreeViewItemFolder �� 
TargetType �� Triggers �� Orientation  TreeViewItemExecutable �� Path  PanelButton  StackPanelButtonPanel  ImageButtonPanel  TextButtonPanel 
 Color2��%   &	        &	 �    7   &	 �    &	 �    &	 �    &	     &	 {    '�� �    '��     'm� 3    &	 �    �� D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    ,s&clr-namespace:System;assembly=mscorlib  :x,http://schemas.microsoft.com/winfx/2006/xaml     " Q�.R��  �� $
��Auto �$��19���� $
��Auto ��� $��* �	 �� $
��Auto ��� .+�d� $��Expander��$
 FalseY�$y�Hidden=�$ Press=��� $��Bd��#��z��# z��#��z��#��z�� . $��1��._��� $��PART_Header��#��z��#��z��# z $��Header��#��z��# z �� $��	ItemsHost��$��1��$��1��$��2�� P�  �@Q�
IsExpanded. ��� $ 	ItemsHost�� � y� �� $ Visibility.Collapsed�� ��	False��P�  �@��HasItems. ��� $ Expander�� � y� �� $ Visibility.Hidden�� ��	False���� " Q� P�  �@Q�
IsSelected . . ���  �@Q�
IsExpanded .  .���  � ��8   ��  �@Q�
IsExpanded .  ��  � �� �� .R��� $ 
Horizontal=�.+��� $	��img��$��15��$��15��$��Uniform=�$��Pictures/folder.ico���� $	��5,0q����� �� " Q�.���  � �� �� " Q�.R��� $ 
Horizontal=�$��20,0,0,0q�.+��� $��15��$��15������ $
 Icon��� $��5,0,0,0q����� $
 Name��� " ��.���  � ��$ 28����  � ��$
 Left=���  � ��$	 Top=���  � ��$ 16(���  � ��$ 0q���  � �� Z� ��  � ��$ Global User Interface)���  �@�FocusVisualStyle Z� ��  � |� .  ��  � ��
 ���� �� " ��.���  � ��$ 28����  � ��$	 200����  � ��$
 Left=���  � ��$ Center=��� " ��.���  � ��$ 28����  � ��$ 28����  � ��$
 Left=���  � ��$ Center=��� " ��.���  � ��$
 Left=���  � ��$ Center=���  � ��$ 10,0,0,0q��� " ��.���  � ��$	 Top=���  � ��$
 Left=��� " ��.���  � ��# � ���� " m�.���  � ��# � ����  � ��$ 0q���  � ��$	 260����  � ��$ 0,0,0,0q���	#FF67A7BD����
� .���  � Color$ Black���  
 Color1# �   � Offset$ 1Q�� # �  $ 0.482Q�.��� -   $��0,0,0,0q�$
��Left=�.+�R� -    R�Name$ TreeViewStartMenuExplorer��$	��320��$	��295��$��	10,10,0,0q�$
��Left=�$	��Top=�$	��Top=�$��Disabled=��� -   $��TextBoxExecute��$
��Left=�$��19��$��
10,335,0,0q� �� TextWrapping$
 Wrap=�$�� ��$	��Top=�$	��295��$��Global User Interface)�$��14(�$��1���� $��310,10,10,70q�.+��� -   $��LabelUserName��$��Center=�$	��Top=� �� TextAlignment$ Center=�$��	User Name��$��24(�$��Global User Interface)�$��Italic$�$��Expanded&��� -   $��GroupBoxUserFolders��$��User's folders��#��� ��.���� $ Vertical=�$��Stretch=�.+��� -     ��Name$  
ButtonUser��#��[ .���� #��[ $ 
Horizontal=�.+��� $��Pictures/user.png��#��[ �� -   ! ��Name$! tbButtonUser��$
��User��#��[ �� -	   $  ButtonMyDocuments��#��[ .���� #��[ $ 
Horizontal=�.+��� $��Pictures/documents.png��#��[ �� $��	Documents��#��[ �� -
   $  ButtonMyPictures��#��[ .���� #��[ $ 
Horizontal=�.+��� $��Pictures/pictures.ico��#��[ �� $��Pictures��#��[ �� -   $  ButtonMyMusic��#��[ .���� #��[ $ 
Horizontal=�.+��� $��Pictures/ringtones.ico���� $��Music��#��[ �� $��Computer Management��#��� ��.���� $ Vertical=�.+��� -   $  ButtonComputer��#��[ .���� #��[ $ 
Horizontal=�.+��� $��Pictures/computer.jpg���� $��Computer��#��[ �� -   $  ButtonPanelConfig��#��[ .���� #��[ $ 
Horizontal=�.+��� $��Pictures/settings.png���� $��Panel Configuration��#��[ �� -   $  ButtonDeviceManagemenet��#��[ .���� #��[ $ 
Horizontal=�.+��� $%��Pictures/computermanagement.png���� $��Device Management��#��[ �� $ 
Horizontal=�$��419,326,10,10q�.+��� -   $  ButtonShutdown��$��Shutdown��$
��Left=�$��24��$	��Top=�$��83���� -   " ��Name$" ComboBoxShutdownOptions��$
��Left=�$��24��$	��Top=�$��20��.@�m� -   $��Log off��m� -   $��Restart��m� -   $��	Hibernate��m� -   $��Standby��m� -   $&�� Shuting down with faster restart��