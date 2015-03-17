<html>
    <head>
        <meta charset="UTF-8">
        <title>FillIn vragenlijst</title>
        <link rel="stylesheet" href="../layout/main.css">
        <link rel="stylesheet" href="../layout/dark.css">
    </head>
    <body>
        <?php
        /// START INCLUDES AND REQUIRES \\\
        
        require '../res/db.php';
        
        /// START FUNCTION DECLARATION \\\
        
        /**
         * Checks whether or not the questionnaire specified by the id and hash
         * parameters exists or not. If the questionnaire exists, $_POST['hid']
         * will be set for the duration of the session. The current value of 
         * $_POST['hid'] will be lost upon calling this function.
         * @param type $id The id of the questionnaire.
         * @param type $hash The hash of the questionnaire.
         * @return boolean True if the questionnaire exists, false otherwise.
         */
        function checkHash($id, $hash){
            return true;
        }
        
        /**
         * Returns the next unanswered question of the given questionnaire
         * @param type $id
         */
        function getNextUnansweredQuestion($id, $hash){
            
        }
        
        /// START MAIN SCRIPT \\\
        
        // CONNECT TO DATABASE \\
        $conn = new mysqli($DBHOST, $DBUSER, $DBPASS);
        
        if($conn->connect_error)
            $db = false;
        else{
            $conn->select_db($DBNAME);
            $db = true;
        }
        
        
        // AUTH USER \\
        
        // If we do not have a database connection, we can not proceed.
        if(!$db)
            $pass = false;
        // If no uid is set, we can not proceed.
        else if(!isset($_GET['uid']))
            $pass = false;
        // If we already have an open session with the current uid, we can
        // simply continue that session.
        else if(isset($_POST['hid']) && $_POST['hid'] == $_GET['uid'])
            $pass = true;
        // If we do not have an open session, yet we do have a uid, we need to
        // check the validity of the hash. This will also create a user session.
        else if(isset($_GET['hash']) && checkHash($_GET['uid'], $_GET['hash']))
            $pass = true;
        // If we do not have an open session, and either the hash is not set or
        // the hash lookup fails, we are not authorized to access the questions.
        else
            $pass = false;
        ?>
        
        <header>
            <nav>
                <div class="navcontainer">
                    <a href="../">
                        <img class="navimg" src="../res/homebutton.png" alt="home">
                    </a>
                </div>
                <div class="navcontainer">
                    <a href="../about/">
                        <img class="navimg" src="../res/aboutbutton.png" alt="home">
                    </a>
                </div>
            </nav>
            <div class="titlecontainer">
                <a href="../">
                    <h1>
                        Fancy Medisch Bedrijf
                    </h1>
                </a>
            </div>        
        </header>
        
        <?php
            // If we can not connect to the database, we display an error.
            if(!$db){
        ?>
        
        <div class="errorbox">
            <h2 class="errortitle">
                Database error
            </h2>
            <p class="error">
                Er kan geen verbinding gemaakt worden met de database. Probeer
                later opnieuw.
            </p>
        </div>
        
        <?php
            }
        
            // If we are not allowed to pass, we display an error.
            else if(!$pass){ 
        ?>
        
        <div class="errorbox">
            <h2 class="errortitle">
                Niet gevonden
            </h2>
            <p class="error">
                Deze vragenlijst bestaat niet.
            </p>
        </div>
               
        <?php 
            }
            
            // If we are allowed to pass, we display the next question.
            else if($pass){
                
            }
        ?> 
         

    </body>
</html>
