   M S B A M L   `   `   `  ����H  DEXEIntegrator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:EXEIntegratorEXEIntegrator  5   	   !    EXEIntegrator.LoadingWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:EXEIntegrator     5   	   -   5          �Name$  Window��5   	    � Title$ EXEInterator��5   	   $	��112��6   $	��512��6+    � 
ResizeMode$ NoResize=�67    � WindowStyle$
 None=�6M    � AllowsTransparency .  � Icon$ Resources/icon.ico��6z   ��5
      �� 5   
    �� Opacity$ 0Q�6    �� Color$ White��6&   .��5      � $
��Left=�6   $	��112��6&   $	��Top=�63   $	��512��6K   .+�5   
   �� -   5         ��Name$ 
Background��5      $	��112��6$   $	��Top=�61   $
��Left=�6I   $	��512��6d   $��/Resources/background.png��6p   6�   � 5   
   -   5        	 �Name$	 
LoadingBar��5      $��16,48,16,16q�6*   ���	#FF343A40
 � Value$
 0Q�6V      
LoadingBar#���   6`   �� 5   
   -   5         ��Name$ LoadingText��5      $��Loading��6%   $
��Left=�67   $��	16,48,0,0q�6R   $	��Top=�6e   $��48��6}   $	��480��6�   
�������$��Center=�6�   $��Center=�6�   $��16(�6�   $��	Open Sans)�6�   5      5      