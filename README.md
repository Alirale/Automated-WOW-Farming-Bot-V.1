<br/><br/>![WoW](https://user-images.githubusercontent.com/59726045/137293657-5d84bfd5-0df8-44c8-9622-bd988cc4fac2.png)<br/>
<br/>
<br/>
<br/>
This repository has an application that is used to automate a procedure in some games called farming.
For farming, you should kill some NPC in-game and loot them by using a toy.
to make all that works automatic, we should gather some data from the game such as (X, Y) position, facing angle in-game map, money amount of the character, the character name and etc.
<br/>
<br/>
Gathering those data from game memory data stored in ram is almost impossible due to blizzard's efforts to encrypt data and hiding them and even if you could reach those memories, you definitely would be banned from the game.
<br/>
<br/>
One of the best ways to avoid getting banned because of accessing game memories is using the ability to create an in-game add-on using the LUA language.
<br/>
<br/>
In the add-on we get the desired data in an infinity loop, then we use a technique to convert those data to colors and then show those colors in the boxes to the top left of the screen. So whenever data changes in-game, the colors change too.
<br/>
<br/><br/><br/>
This is how I Convert data to color:<br/><br/>
![wowbot_xy](https://user-images.githubusercontent.com/59726045/137365865-8fc15df1-409c-424c-9539-83d02c8a2d48.png)

<br/>
<br/>
First, I had to cut the number to 5 digits, then by converting that binary number to the hexadecimal I got a hexadecimal number like "1048c".
In order to convert that hexadecimal number to a color, I just add a zero before the number.<br/>
After that I had a "#01048c" Color! and the last step was converting that number to that specific box color.
Also, when I wanted to get the data from color For the Bot App, I just scanned that color, and by converting that hexadecimal number to decimal I could collect the data that the bot application needed.
<br/>
<br/>
<br/><br/><br/>
After getting those data by scanning colors of those boxes the procedure begins...
