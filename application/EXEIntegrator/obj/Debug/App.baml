   M S B A M L   `   `   `  ����H  DEXEIntegrator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:EXEIntegratorEXEIntegrator  5          EXEIntegrator.App   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      -localclr-namespace:EXEIntegrator     5        �� 	Resources  5         ProgressBar.Progress �� Color  ProgressBar.Background  
LoadingBar �� 
TargetType �� Property �� Value �� 
TargetType �Name'  !System.Windows.VisualStateManager   VisualStateGroups%  System.Windows.VisualStateGroup   States   System.Windows.VisualState	   
Storyboard
 g� RepeatBehavior �� TargetProperty �� 
TargetName9  3System.Windows.Media.Animation.EasingDoubleKeyFrame O� KeyTime O� Value8  2System.Windows.Media.Animation.EasingPointKeyFrame $� KeyTime $� Value ��Name �� RadiusX �� RadiusY J� RenderTransformOrigin �� Triggers P� Property P� Value �� 
TargetName �� Angle  FocusVisual J� SnapsToDevicePixels �� StrokeDashArray  RedButton.Static.Background " RedButton.MouseOver.Background   RedButton.Pressed.Background  	RedButton ��Name �� CornerRadius % !WindowInteractionRed.Static.Color (	 $WindowInteractionRed.MouseOver.Color &
 "WindowInteractionRed.Pressed.Color  WindowInteractionRed��%F  &	        &	 #     &	 F     7   7  &	 �    &	 h    &	 �    &	 �    &	 �    7  7  7  &	 �
    &		 �
    &	
 �
    &	 �
    7  7 	 7 	 7 	 �� 5   
   $ 	#FF28A745��67   �� 5	   
   $ 	#FF343A40��69   �� 5   
   " �6#   .�5      ��  � ��8   6+   �� 5       � ��8  6+   �� 5       � �� 5      �� 5      " �6&   .R�5      � $ TemplateRoot��6    5        5   "   CommonStates��. 5   &     Determinate��62     5   &   Indeterminate��.	 5   *   �� $
 Forever��65   .��5   .   U� $V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�6L   $ 	Animation��6�   .\�5   2     $ 0��6G   $
 0.25Q�6S     5   2   $ 0:0:1��6G   $
 0.25Q�6W     5   2   $ 0:0:2��6G   $
 0.25Q�6W   5   /   *� 5   .   $' !(UIElement.RenderTransformOrigin)�6K   $ 	Animation��6�   .'�5   2     $ 0��6F   $ -0.5,0.5&�6R     5   2   $ 0:0:1��6F   $ 0.5,0.5&�6V     5    2   $ 0:0:2��6F   $ 1.5,0.5&�6V   5!   /   5"   +   5#   '   5$   #   .+�5&      �� $ 
PART_Track��6(   #��z��6<   $ 8��6`   $ 8��6l   � 5'      $ PART_Indicator��6#   }�. $
��Left=�6O   .+�5(   "   �� $ 	Indicator��6,   #��z��6?   $ 8��6c   $ 8��6o   �� 5)   "   $ 	Animation��6,   #��z��6?   $ 0.5,0.5&�6c   $ 8��6�   $ 8��6�   z�5*   &   V� 5+   *   .~�5,   .   �� �� 5-   .   �� 5.   .   T� 5/   .   50   +   52   #   53      54       55      P� 56       �@�Orientation$ Vertical=�6=   . �57   "   �� $ TemplateRoot��6D    �@�LayoutTransform 58   &   �� 59   *   $	 -90Q�6:   5;   #   5<      P� 5=       �@�IsIndeterminate . . �5>   "   �� $ 	Indicator��6?    � y�$ 	Collapsed=�6V   5?      5A      5C      5D      �� 5E   
   .�5F      ��  � �� 5G      �� 5H      .R�5I      �� $��2q�6$    . #��� ��6J   $��1��6�   $	 1 2R�6�   5J      5L      5M      �� 5O   
   $ 	#FFDC3545��6>   �� 5P   
   $ 	#FFBA2452��6A   �� 5Q   
   $ 	#FFB32767��6?   �� 5S   
   " ��6"   .�5U      ��  � ��8   6+   �� 5V       � ��
 �����6+   �� 5W       � ��$ button��6(   �� 5X       � �� 5Y      �� 5Z      " ��6&   .R�5[      �� $ border��6!   $ 8��61   #��z��6B    . $��0,0,0,0q�6�   $��48��6�   $	��112��6�   ._�5\      �� #��z��6(   $��Center=�6I   $��Center=�6f    . |�.  $��Open Sans SemiBold)�6�   $��16(�6�   #��z��6�   6  5^       5_      P� 5`       �@J�IsMouseOver . . �5a   "   �� $ border��6?   8   � ��6S   5b      P� 5c       � �� . . �5d   "   �� $ border��6?   8   � ��6S   5e      5g      5i      5j      �� 5m   
   $ 	#FFDC3545��6D   �� 5n   
   $ 	#FFBA2452��6G   �� 5o   
   $ 	#FFB32767��6E   �� 5s   
   " ��6-   .�5t      ��  � ��8   6+   �� 5w       � �� 5x      �� 5y      " ��6&   .R�5z      A� $��circle��6"   $
��Auto��60   $
��Auto��6=   $��0q�6K   8�� 6V    5{      P� 5|       �@J�IsMouseOver . . �5}   "   �� $ circle��69   8   � ��6M   5~      P� 5       � �� . . �5�   "   �� $ circle��69   8   � ��6M   5�      5�      5�      5�      5�      5�      