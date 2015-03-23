<html>
    <head>
        <meta charset="UTF-8">
        <title>FillIn vragenlijst</title>
        <link rel="stylesheet" href="../layout/main.css">
        <link rel="stylesheet" href="../layout/dark.css">
    </head>
    <body>
        <?php
        /// START GLOBAL INCLUDES AND REQUIRES \\\
        
        require '../db/db.php';
        
        /// START CLASS DECLARATION \\\
        
        class question
        {
            public $id;
            public $title;
            public $text;
        }
        
        
        /// START FUNCTION DECLARATION \\\
        
        /**
         * Checks whether or not the client specified by the id and hash
         * parameters exists or not. If the client exists, $_POST['hid']
         * and $_POST['hhash'] will be set for the duration of the session. 
         * The current value of $_POST['hid'] and $_POST['hhash'] will be lost 
         * upon calling this function.
         * @param $id The id of the client.
         * @param $hash The hash of the client.
         * @param mysqli $dbconn The database connection.
         * @return True if the client exists; false otherwise.
         */
        function checkHash($id, $hash, mysqli $dbconn){
            require '../db/qexists.php';
            
            $stmt = $dbconn->prepare($EXISTSQUERY);
            $stmt->bind_param("is", $id, $hash);
            $stmt->execute();
            $result = $stmt->get_result();
            
            if($result === FALSE || !is_object($result) || $result->num_rows <= 0)            
                return false;
            
            $_POST['hid'] = $id;
            $_POST['hhash'] = $hash;
            return true;
        }
        
        //TODO: ADD START
        /**
         * Sets the variables $GLOBALS['done'] and $GLOBALS['start'] if the 
         * client is done with his questionnaire or if he is at the start of it,
         * respectively. Will set both of these to TRUE if the requested client
         * does not exist.
         * @param $id The id of the client.
         * @param $hash The hash of the client.
         * @param mysqli $dbconn The database connection.
         */
        function getStartOrDone($id, $hash, mysqli $dbconn){
            require '../db/qstartdone.php';
            
            $stmt = $dbconn->prepare($STARTDONEQUERY);
            $stmt->bind_param("is", $id, $hash);
            $stmt->execute();
            $result = $stmt->get_result();
            
            // ERROR STATE: CLIENT DOES NOT EXIST
            if($result === FALSE || !is_object($result) || $result->num_rows <= 0)            
            {
                $GLOBALS['done'] = true;
                $GLOBALS['start'] = true;
                return;
            }            
            
            $row = $result->fetch_assoc();
            
            $GLOBALS['done'] = $row['done'];
            $GLOBALS['start'] = false;
        }
        
        /**
         * Returns the next unanswered question of the given questionnaire.
         * @param $id The id of the client.
         * @param $hash The hash of the client.
         * @param mysqli $dbconn The database connection.
         * @return question An object containing the title, the text and the id 
         * of the first unanswered question. Will be NULL if there are no more
         * questions.
         */
        function getNextUnansweredQuestion($id, $hash, mysqli $dbconn){
            require '../db/qnext.php';
            
            $stmt = $dbconn->prepare($NEXTQUERY);
            $stmt->bind_param("is", $id, $hash);
            $stmt->execute();
            $result = $stmt->get_result();
            
            if($result === FALSE || !is_object($result) || $result->num_rows <= 0)                    
                return null;
            
            $row = $result->fetch_assoc();
            
            $output = new question();
            $output->id = $row['id'];
            $output->title = $row['title'];
            $output->text = $row['text'];
            
            return $output;
        }
        
        function printDone(){
            ?>
        <!--Temporarily an errorbox; change to something prettier later-->
        <div class="errorbox">
            <h2 class="errortitle">
                Gefeliciteerd!
            </h2>
            <p class="error">
                U heeft de vragenlijst successvol vervolledigd.
            </p>
        </div>
            <?php
        }
        
        /**
         * Sets the 'done' value of the client with id $id and hash $hash to
         * true.
         * @param $id The id of the client.
         * @param $hash The hash of the client.
         * @param mysqli $dbconn The database connection.
         */
        function setDone($id, $hash, mysqli $dbconn){
            require '../db/qsetdone.php';
            
            $stmt = $dbconn->prepare($SETDONEQUERY);
            $stmt->bind_param("is", $id, $hash);
            $stmt->execute();
        }
        
        /// START MAIN SCRIPT \\\
        
        // CONNECT TO DATABASE \\
        
        $conn = new mysqli($DBHOST, $DBUSER, $DBPASS, $DBNAME, $DBPORT);
        
        if($conn->connect_error)
            $db = false;
        else{
            $conn->select_db($DBNAME);
            $db = true;
        }
        
        // AUTH USER \\
        
        // If we do not have a database connection, we can not proceed.
        if(!$db) {
            $pass = false;
        }
        // If no uid is set, or the uid is not a number or if the hash is not 
        // set, we can not proceed.
        else if(!isset($_GET['uid']) || !is_numeric($_GET['uid']) || !isset($_GET['hash']))
            $pass = false;
        // If we already have an open session with the current uid and hash, 
        // we can simply continue that session.
        else if(isset($_POST['hid']) && $_POST['hid'] == $_GET['uid'] && isset($_POST['hhash']) && $_POST['hhash'] == $_GET['hash'])
            $pass = true;
        // If we do not have an open session, yet we do have a uid and a hash, 
        // we need to check the validity of the hash. 
        // This will also create a user session.
        else if(checkHash($_GET['uid'], $_GET['hash'], $conn))
            $pass = true;
        // If the hash lookup fails, we are not authorized to access the questions.
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
            // If we are allowed to pass,
            else{
                // we check whether we are done or perhaps just beginning.
                getStartOrDone($_POST['hid'], $_POST['hhash'], $conn);
                
                // If we are done, we display an appropriate message.
                if($GLOBALS['done']){
                    printDone();
                }
                // Same for if we are beginning.
                else if($GLOBALS['start']){
                    //TODO: IMPLEMENT
                } 
                // If we are not done, and not starting, we fetch the next 
                // question.
                else {
                    $question = getNextUnansweredQuestion($_POST['hid'], $_POST['hhash'], $conn);
                    // If we didn't get a new question, that means we're done.
                    // We update the database and display an appropriate message.
                    if(is_null($question)){
                        setDone();
                        printDone();
                    }
                    // Else we finally display the question.
                    else{
                        ?>
        
        <!--Temporarily an errorbox; change to something prettier later-->
        <div class="errorbox">
            <h2 class="errortitle">
                <?php echo($question->title);?>
            </h2>
            <p class="error">
                <?php echo($question->text);?>
            </p>
        </div>
                        <?php
                    }
                }
                
                
                    
                    
                
               
            }
        ?> 
         

    </body>
</html>
