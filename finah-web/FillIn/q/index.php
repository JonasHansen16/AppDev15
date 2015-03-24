<?php
    /// BEGIN SESSION \\\
    session_start();
?>

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
         * parameters exists or not. If the client exists, $_SESSION['hid']
         * and $_SESSION['hhash'] will be set for the duration of the session. 
         * The current value of $_SESSION['hid'] and $_SESSION['hhash'] will be lost 
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
            
            $_SESSION['hid'] = $id;
            $_SESSION['hhash'] = $hash;
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
        
        /**
         * Inserts an answer into the database, invalidating all other answers
         * for the same question by the same user by setting their 'final' value
         * to false.
         * @param type $id The id of the client.
         * @param type $hash The hash of the client.
         * @param mysqli $dbconn The database connection.
         * @param type $qid The id of the question to insert.
         * @param type $score The score of the question to insert.
         * @param type $help Whether or not the client has requested aid.
         */
        function insertAnswer($id, $hash, mysqli $dbconn, $qid, $score, $help){
            require '../db/qinsertanswer.php';
            require '../db/qinvalidateanswers.php';
            
            // CURRENTLY THIS WORKS WITH SIMPLE UPDATES AND INSERTS
            // TODO: ADD SECURITY AND SAFETY CHECKS TO QUERIES
            // THIS WILL HAVE TO BE DONE WHILE WRITING THE API
            
            // First we invalidate all other answers by this client for this 
            // question.
            $updstmt = $dbconn->prepare($INVALIDATEANSWERSQUERY);
            $updstmt->bind_param("ii", $id, $qid);
            $updstmt->execute();
            
            // Then we insert our new answer.
            $insstmt = $dbconn->prepare($INSERTANSWERQUERY);
            $insstmt->bind_param("iiii", $id, $qid, $score, $help);
            $insstmt->execute();
        } 
        
        /**
         * Validates the input $score and $help following these rules:
         * $score must be numeric and be within the range [1,5]
         * $help must be numeric and be within the range [0,1]
         * @param type $score The first variable to validate.
         * @param type $help The second variable to validate.
         * @return boolean True if all of the input validated according to the
         * rules, false otherwise.
         */
        function validateAnswerInput($score, $help){
            if(!is_numeric($score) || !is_numeric($help))
                return false;
            
            if($score < 1 || $score > 5)
                return false;
            
            if($help < 0 || $help > 1)
                return false;
            
            return true;
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
        else if(isset($_SESSION['hid']) && $_SESSION['hid'] == $_GET['uid'] && isset($_SESSION['hhash']) && $_SESSION['hhash'] == $_GET['hash'])
            $pass = true;
        // If we do not have an open session, yet we do have a uid and a hash, 
        // we need to check the validity of the hash. 
        // This will also create a user session.
        else if(checkHash($_GET['uid'], $_GET['hash'], $conn))
            $pass = true;
        // If the hash lookup fails, we are not authorized to access the questions.
        else
            $pass = false;
        
        // If we are authorized, and we have an answer to submit and that answer is valid
        if($pass && isset($_SESSION['hqid']) && isset($_POST['primaryanswer']) && isset($_POST['secondaryanswer']) && validateAnswerInput($_POST['primaryanswer'], $_POST['secondaryanswer']))
            // We submit the answer to the database
            insertAnswer($_SESSION['hid'], $_SESSION['hhash'], $conn, $_SESSION['hqid'], $_POST['primaryanswer'], $_POST['secondaryanswer']); 
        
        // Regardless of whether the answer was submitted or not, we unset the data.
        unset($_POST['primaryanswer']);
        unset($_POST['secondaryanswer']);
        unset($_SESSION['hqid']);
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
                getStartOrDone($_SESSION['hid'], $_SESSION['hhash'], $conn);
                
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
                    $question = getNextUnansweredQuestion($_SESSION['hid'], $_SESSION['hhash'], $conn);
                    // If we didn't get a new question, that means we're done.
                    // We update the database and display an appropriate message.
                    if(is_null($question)){
                        setDone($_SESSION['hid'], $_SESSION['hhash'], $conn);
                        printDone();
                    }
                    // Else we set our hidden question id 
                    // and finally display the question
                    else{
                        $_SESSION['hqid'] = $question->id;
                        ?>
        
        <!--Temporarily an errorbox; change to something prettier later-->
        <div class="questionbox">
            <h2 class="questiontitle">
                <?php echo($question->title);?>
            </h2>
            <p class="questiontext">
                <?php echo($question->text);?>
            </p>
            <!-- TODO: IMPLEMENT
                <img class="questionimage"></img>
            -->
            <!-- TODO: IMPLEMENT
                <progress value="22" max="100"></progress>
            -->
            <form action="index.php?uid=<?php echo $_SESSION['hid']; ?>&hash=<?php echo $_SESSION['hhash']; ?>" method="post">
                
                <p>Hoe ervaart u dit onderdeel?</p>
                <div class="answercontainer" id="primary">
                    <label class="answerboxlabel" onclick="hide()">
                        <input type="radio" name="primaryanswer" value="1" />
                        <div class="answerbox">
                            <p class="answerboxtext">
                                Verloopt naar wens
                            </p>
                        </div>
                    </label>
                    <label class="answerboxlabel" onclick="hide()">
                        <input type="radio" name="primaryanswer" value="2" />
                        <div class="answerbox">
                            <p class="answerboxtext">
                                Probleem - niet hinderlijk
                            </p>
                        </div>
                    </label>
                    <label class="answerboxlabel" onclick="show()">
                        <input type="radio" name="primaryanswer" value="3" />
                        <div class="answerbox">
                            <p class="answerboxtext">
                                Probleem - hinderlijk voor cliënt
                            </p>
                        </div>
                    </label>
                    <label class="answerboxlabel" onclick="show()">
                        <input type="radio" name="primaryanswer" value="4" />
                        <div class="answerbox">
                            <p class="answerboxtext">
                                Probleem - hinderlijk voor mantelzorger
                            </p>
                        </div>
                    </label>
                    <label class="answerboxlabel" onclick="show()">
                        <input type="radio" name="primaryanswer" value="5" />
                        <div class="answerbox">
                            <p class="answerboxtext">
                                Probleem - hinderlijk voor beide
                            </p>
                        </div>
                    </label>
                </div>
                
                <p class="visible">Wilt u dat we hieraan werken?</p>
                <div class="answercontainer" id="secondary">
                    <label class="answerboxlabel visible">
                        <input type="radio" name="secondaryanswer" value="1" />
                        <div class="answerbox">
                            <p class="answerboxtext">
                                Ja
                            </p>
                        </div>
                    </label>
                    <label class="answerboxlabel visible">
                        <input type="radio" name="secondaryanswer" value="0"  checked="checked"/>
                        <div class="answerbox">
                            <p class="answerboxtext">
                                Nee
                            </p>
                        </div>
                    </label>
                    
                    <input type="submit" class="submitbutton" value="Volgende vraag">
                </div>
                
            </form>
        </div>
                        <?php
                    }
                }  
            }
        ?> 
         
        <script>
            // Hides the help section
            function hide(){
                var toHide = document.getElementsByClassName("visible");
                for(element in toHide){
                    element.classList.remove("visible");
                    element.classList.add("hidden");
                }
            }
            
            // Shows the help section
            function show(){
                var toHide = document.getElementsByClassName("hidden");
                for(element in toHide){
                    element.classList.remove("hidden");
                    element.classList.add("visible");
                }
            }
            
            //Hide the help section
            hide();
        </script>
    </body>
</html>
