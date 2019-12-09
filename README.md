# Emotiv-EEG-Unity
Visualize the raw average band power of EEG in Unity, based on Emotiv community SDK

## 1 Overview
This Unity project was built to read the Average Band Power from each electrode on the Emotiv Epoc+ headset. It was based on the Unity project in Emotiv SDK Community Edition which does not provide an API for reading the raw data from the headset. Therefore, I ported the C# version API into this new project so that you can play with these data and visualize them in Unity. There are also two demo scene you can play with.
 
## 2 System requirements
 - Unity 2019.2 or later
- Emotiv BCI
- Epoc+ Neuroheadsets

## 3 Intro to the Average Band Power of EEG data
There are 14 electrode on Emotiv Epoc+. The API provides the reading of average power of distinct frequency band, including delta (0.5–4 Hz), theta (4–8 Hz), alpha (8–12 Hz), beta (12–30 Hz), and gamma (30–100 Hz), also can be referred as average band power. These data are decomposed from the raw signal of the electrode.
 
## 4 Demo Scenes
- Bar Visualization
  ![Bar Visualization Demo](/bar-visualization-demo.png "Bar Visualization Demo")
  In this demo scene, the data values are visualized using the length of bars. Each bar represents a electrode on the headset. You can select a specific frequency band for visualization by clicking the buttons in the upper right corner.
  Demo Video: https://vimeo.com/373335344 
- Sphere Visualization
  ![Sphere Visualization Demo](/sphere-visualization-demo.png "Sphere Visualization Demo")
  In this demo scene, the data values are visualized using the altitudes, scales and colors of the spheres. Each sphere represents a electrode on the headset. The altitudes are affected by the alpha band. The low beta band controls the scale of the sphere. The high-beta band controls the color hue and theta controls the color saturation.
Demo Video: https://vimeo.com/373335370

## 5	Possible Issues
- Headset contact quality low
  Usually, a contact quality of 80% is good enough. Adjust electrode positions on your skull to improve the contact quality. Make sure there is enough fluid on each electrode.
- No data reading in the Demo scenes on second run
  It seems occuring when using the Bluetooth as connection. It might be caused by the original API from Emotiv which I don't have a perfect solution for it. One alternative solution is to stop current session and restart the Unity project and it should work again. Another would be mannually turn off the 'Controller' script after the first run by uncheck the small box. 
- Data reading value lower or higher than expected
The brain wave data are different for each person, so the scale of the number setup in the demo scenes might not be suitable for everyone. You can adjust them yourself.
  
 
## 6 Resources
- Emotive SDK Community Edition  https://github.com/Emotiv/community-sdk
- Average Band Power  https://raphaelvallat.com/bandpower.html

