<br/>
<br/>
![WoW](https://user-images.githubusercontent.com/59726045/137293074-fd9e5c43-1d35-4467-b8e0-402432e4992c.png)

<br/>
<br/>
<br/>
<br/>
This repository has an application that is used to automate a procedure in some games called farming.
For farming, you should kill some NPC in-game and loot them using a toy.
to make all that works automatic, we should gather some data from the game such as (X, Y) position, facing angle in-game map, money amount of the character, the character name and etc.
<br/>
<br/>
gathering those data from game memory data stored in ram is almost impossible due to blizzard's efforts to encrypt data and hiding them and even if you could reach those memories, you definitely would be banned from the game.
<br/>
<br/>
one of the best ways to avoid getting banned because of accessing game memories is using the ability to create an in-game add-on using the LUA language.
<br/>
<br/>
In the add-on we get the desired data in an infinity loop, then we use a technique to convert those data to colors and then show those colors in the boxes to the top left of the screen. So whenever data changes in-game, the colors change too.
<br/>
<br/>
This is how I Convert data to color:<br/><br/>
<br/>
<br/>
![wowbot_xy](https://user-images.githubusercontent.com/59726045/137293389-3c93056b-5da3-42d0-a84e-0b5f98fd2cc3.png)

<br/>
<br/>
first, we should cut the number to 5 digits, then by converting that binary number to the hexadecimal and when we want to get the data from color for the Bot App, we just scan that color, and by converting its hexadecimal number to decimal we collect data in the bot application
<br/>
<br/>
<br/>
After getting those data by scanning colors of those boxes the procedure begins...
