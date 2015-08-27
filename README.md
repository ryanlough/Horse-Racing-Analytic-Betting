#Horse Race Analytics

The goal of this project is to find useful trends in historical horse racing data in the hopes that it can be used to have an automatic bet system with a net positive source of income.

##This project will have three parts:

1. ~~Collect and store historical race data.~~
2. **Analyze and find trends in this data.**
3. Create an automated betting system to bet based on this analysis.

###Collect and store historical race data - COMPLETE
Using C# to pull the data out of online PDFs, Protobuf-net to serialize this data into a storable binary form, and SQLite as my database for easy and quick storage / access.

###Analyze and find trends in this data - IN PROGRESS
With C# and the SQLite database created in the previous part of this project, I will attempt to find trends in these areas:

1. Which professional selectors have the best odds (eg kristpicks)? Are their predictions affected by the weather, track type, etc?
2. What type of bet has the best monetary return? (Win, Place, Show, Exacta, Trifecta, Superfecta) What combination of horses is the best for each type of bet?
3. Do certain owners, trainers, jockeys, horses work best together?

###Create an automated betting system to bet based on this analysis. - TODO
Depending on the quality of the trends found in the previous part of this project, set up an automatic betting system to bet in an intelligent manner year-round. If this is a success, move to different tracks.

####Lessons learned so far:

1. Being 95% sure that a website doesn't have a CAPTCHA is not good enough.
2. Backup your data. All your data.
3. Bad things can happen if you don't dispose of your variables when you're done with them.
