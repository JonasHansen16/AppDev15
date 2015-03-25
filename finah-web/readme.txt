REQUIREMENTS:
-MySQL server (username, password, hostname, database and port set in FillIn/db/db.php)
-Database generated via script in the scripts folder
-mysqli PHP extension (see http://php.net/manual/en/mysqli.installation.php)

How to run:
Simply set up your preferred PHP server with the source files contained in the folder FillIn, and then run it. 
Visit it via your web browser.

The homepage and the about page do not contain any PHP.
In the root of the file structure (FillIn/) you can find a file called test.php. This file can be used to test your mysqli installation.
In order to test the questionnaire, you must visit /q/index.php?uid=XXX&hash=YYY 
where XXX stands for the id of a user, and YYY stands for the hash of that user.
