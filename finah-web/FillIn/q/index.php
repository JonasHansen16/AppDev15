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
    <body onload="start()" class="preload">
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
        
        class introduction
        {
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
            
            $GLOBALS['start'] = $row['start'];
            $GLOBALS['done'] = $row['done'];
        }
        
        /**
         * Returns the next unanswered question of the given client.
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
        
        /**
         * Returns the intro of the questionnaire of the given client.
         * @param $id The id of the client.
         * @param $hash The hash of the client.
         * @param mysqli $dbconn The database connection.
         * @return introduction An object containing the title and the 
         * introduction text of the questionnaire. Will be NULL if the client
         * does not exist.
         */
        function getIntro($id, $hash, mysqli $dbconn){
            require '../db/qintro.php';
            
            $stmt = $dbconn->prepare($INTROQUERY);
            $stmt->bind_param("is", $id, $hash);
            $stmt->execute();
            $result = $stmt->get_result();
            
            if($result === FALSE || !is_object($result) || $result->num_rows <= 0)                    
                return null;
            
            $row = $result->fetch_assoc();
            
            $output = new introduction();
            $output->title = $row['title'];
            $output->text = $row['intro'];
            
            return $output;
        }
        
        function printDone(){
            ?>
        <div class="successbox">
            <h2 class="successtitle">
                Gefeliciteerd!
            </h2>
            <p class="success">
                U heeft de vragenlijst successvol vervolledigd.
            </p>
        </div>
            <?php
        }
        
        /**
         * Sets the 'start' value of the client with id $id and hash $hash to
         * true.
         * @param $id The id of the client.
         * @param $hash The hash of the client.
         * @param mysqli $dbconn The database connection.
         */
        function setStart($id, $hash, mysqli $dbconn){
            require '../db/qsetstart.php';
            
            $stmt = $dbconn->prepare($SETSTARTQUERY);
            $stmt->bind_param("is", $id, $hash);
            $stmt->execute();
        }
        
        /**
         * Sets the 'start' value of the client with id $id and hash $hash to
         * false.
         * @param $id The id of the client.
         * @param $hash The hash of the client.
         * @param mysqli $dbconn The database connection.
         */
        function unsetStart($id, $hash, mysqli $dbconn){
            require '../db/qunsetstart.php';
            
            $stmt = $dbconn->prepare($UNSETSTARTQUERY);
            $stmt->bind_param("is", $id, $hash);
            $stmt->execute();
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
         * Sets the 'done' value of the client with id $id and hash $hash to
         * false.
         * @param $id The id of the client.
         * @param $hash The hash of the client.
         * @param mysqli $dbconn The database connection.
         */
        function unsetDone($id, $hash, mysqli $dbconn){
            require '../db/qunsetdone.php';
            
            $stmt = $dbconn->prepare($UNSETDONEQUERY);
            $stmt->bind_param("is", $id, $hash);
            $stmt->execute();
        }
        
        /**
         * Restarts the client's questionnaire by invalidating all his answers.
         * @param $id The id of the client.
         * @param $hash The hash of the client.
         * @param mysqli $dbconn The database connection.
         */
        function resetQuestionnaire($id, $hash, mysqli $dbconn){
            require '../db/qrestart.php';
            
            // CURRENTLY THIS WORKS WITH SIMPLE UPDATES
            // TODO: ADD SECURITY AND SAFETY CHECKS TO QUERIES
            // THIS WILL HAVE TO BE DONE WHILE WRITING THE API
            
            $stmt = $dbconn->prepare($RESTARTQUERY);
            $stmt->bind_param("i", $id);
            $stmt->execute();
        }
        
        function previousQuestion($id, $hash, mysqli $dbconn){
            require '../db/qprevious.php';
            
            // CURRENTLY THIS WORKS WITH SIMPLE UPDATES
            // TODO: ADD SECURITY AND SAFETY CHECKS TO QUERIES
            // THIS WILL HAVE TO BE DONE WHILE WRITING THE APIZ
            $stmt = $dbconn->prepare($PREVIOUSQUERY);
            $stmt->bind_param("ii", $id, $id);
            $stmt->execute();
        }
        
        /**
         * Inserts an answer into the database, invalidating all other answers
         * for the same question by the same user by setting their 'final' value
         * to false. Note that $help will be set to false if $score is lower
         * than three, regardless of its actual value.
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
            
            // If score is lower than three, set help to false.
            if($score < 3)
                $help = 0;
            
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
         * Returns the total amount of questions in the questionnaire of the 
         * given client.
         * @param $id The id of the client.
         * @param $hash The hash of the client.
         * @param mysqli $dbconn The database connection.
         * @return int The total amount of questions in the questionnaire of
         * the given client. Will be 0 if the client does not exist.
         */
        function totalCount($id, $hash, mysqli $dbconn){
            require '../db/qtotalcount.php';
            
            $stmt = $dbconn->prepare($TOTALCOUNTQUERY);
            $stmt->bind_param("is", $id, $hash);
            $stmt->execute();
            $result = $stmt->get_result();
            
            if($result === FALSE || !is_object($result) || $result->num_rows <= 0)                    
                return 0;
            
            $row = $result->fetch_assoc();
            
            $output = $row['amount'];
            
            return $output;
        }
        
        /**
         * Returns the total amount of different valid answers the client has 
         * already given.
         * @param $id The id of the client.
         * @param $hash The hash of the client.
         * @param mysqli $dbconn The database connection.
         * @return int The total amount of different valid answers the client has 
         * already given. Will be 0 if the client does not exist.
         */
        function answerCount($id, $hash, mysqli $dbconn){
            require '../db/qanswercount.php';
            
            $stmt = $dbconn->prepare($ANSWERCOUNTQUERY);
            $stmt->bind_param("is", $id, $hash);
            $stmt->execute();
            $result = $stmt->get_result();
            
            if($result === FALSE || !is_object($result) || $result->num_rows <= 0)                    
                return 0;
            
            $row = $result->fetch_assoc();
            
            $output = $row['amount'];
            
            return $output;
        }
        
        /**
         * Returns the image belonging to a question.
         * @param $id The id of the client.
         * @param $hash The hash of the client.
         * @param mysqli $dbconn The database connection.
         * @param $qid The id of the question.
         * @return int The image of the question, or null if no such question
         * exists or if the user does not have access to that question. WARNING:
         * this image still needs to be encoded in base64.
         */
        function getImage($id, $hash, mysqli $dbconn, $qid){
            require '../db/qgetimage.php';
            
            $stmt = $dbconn->prepare($GETIMAGEQUERY);
            $stmt->bind_param("isi", $id, $hash, $qid);
            $stmt->execute();
            $result = $stmt->get_result();
            
            if($result === FALSE || !is_object($result) || $result->num_rows <= 0)                    
                return NULL;
            
            $row = $result->fetch_assoc();
            
            $output = $row['image'];
            
            return $output;
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
        
        // If we are authorized, and we need to start the questionnaire
        if($pass && isset($_POST['started']))
            // We do so by unsetting start
            unsetStart($_SESSION['hid'], $_SESSION['hhash'], $conn);
        
        // If we are authorized, and we need to go back to the instructions
        if($pass && isset($_POST['instructions']))
            // We do so by setting start
            setStart($_SESSION['hid'], $_SESSION['hhash'], $conn);
        
        // If we are authorized, and we need to restart the questionnaire
        if($pass && isset($_POST['reset']))
            // We do so
            resetQuestionnaire($_SESSION['hid'], $_SESSION['hhash'], $conn);
        
        // If we are authoriwed, and we need to return to the previous question
        if($pass && isset($_POST['previous']))
            // We do so
            previousQuestion($_SESSION['hid'], $_SESSION['hhash'], $conn);
            
        // Unset all the data
        unset($_POST['primaryanswer']);
        unset($_POST['secondaryanswer']);
        unset($_POST['started']);
        unset($_POST['instructions']);
        unset($_POST['reset']);
        unset($_POST['previous']);
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
                    $intro = getIntro($_SESSION['hid'], $_SESSION['hhash'], $conn);
        ?>
        <div class="questionbox">
            <h2 class="questiontitle">
                Instructies
            </h2>
            <div class="intro">
                <div class="introtext">
                    <p class="starttext">
                        Welkom bij uw online vragenlijst! Hieronder volgt eerst even wat
                        uitleg over hoe het invullen van een vragenlijst werkt. Daarna
                        volgt specifieke informatie over de vragenlijst die uw
                        hulpverlener voor u heeft uitgekozen.
                    </p>
                    <p class="starttext">
                        Het invullen van een online vragenlijst is vrij gemakkelijk. Na
                        hieronder op 'Volgende' te klikken, zal u de eerste vraag te 
                        zien krijgen. Neem de tijd om deze op uw eigen tempo te lezen.
                        Er is helemaal geen tijdsdruk.
                    </p>
                    <p class="starttext">
                        Na dat u de vraag hebt gelezen, is het de bedoeling dat u het 
                        meest gepaste van de vijf antwoorden kiest. Dit doet u door
                        simpelweg op het antwoord te klikken. Het huidig gekozen
                        antwoord zal in een andere kleur komen te staan. Wees gerust,
                        indien u per ongeluk op het verkeerde antwoord klikt, kan u
                        dit nog altijd veranderen door simpelweg op een ander antwoord
                        te klikken.
                    </p>
                    <p class="starttext">
                        Indien u aangeeft dat iets hinderlijk is voor u of uw 
                        mantelzorger, dan krijgt u ook de mogelijkheid om te vragen dat
                        hier aan gewerkt moet worden. U kan dit aangeven net zoals u een
                        vraag beantwoordt. Standaard staat dit op 'Nee'.
                    </p>
                    <p class="starttext">
                        Om een vraag in te dienen klikt u simpelweg op de knop 'Volgende
                        vraag'. Indien u per ongeluk een vergissing heeft gemaakt, geen
                        probleem! U kan altijd op 'Vorige vraag' klikken om terug te
                        keren naar de vorige vraag, en deze opnieuw oplossen.
                    </p>
                    <p class="starttext">
                        Verder kan u ook nog volledig opnieuw beginnen met de
                        vragenlijst door op "Herstarten" te klikken. Merk op dat u dan
                        ook nog de bevestiging moet accepteren. Een laatste mogelijkheid
                        is om terug te keren naar dit scherm. Dat doet u door op de
                        'Instructies'-knop te klikken.
                    </p>
                    <?php 
                        if($intro != null) 
                        {
                    ?>
                    <p class="starttext">
                        De titel van de vragenlijst die u moet invullen is 
                        <?php echo $intro->title; ?>.
                    </p>
                    <p class="starttext">
                        <?php echo $intro->text; ?>
                    </p>
                    <?php
                        }
                    ?>
                </div>
                <div class="introbutton">
                    <form action="index.php?uid=<?php echo $_SESSION['hid']; ?>&hash=<?php echo $_SESSION['hhash']; ?>" method="post" class="introform">
                        <input type="radio" name="started" value="1" class="nodisplay" checked="checked"/>
                        <input type="submit" class="submitbutton" value="Volgende">
                    </form>
                </div>
            </div>
        </div>
                        
                        
        <?php
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
                    // Else we set our hidden question id, our current and total
                    // question counts, and then finally display the question
                    else{
                        $_SESSION['hqid'] = $question->id;
                        $currQuestion = answerCount($_SESSION['hid'], $_SESSION['hhash'], $conn) + 1;
                        $total = totalCount($_SESSION['hid'], $_SESSION['hhash'], $conn);
                        $image = getImage($_SESSION['hid'], $_SESSION['hhash'], $conn, $_SESSION['hqid']);
                        ?>
 
        <div class="questionbox">
            <p class="progresstext">
                Vraag <?php echo $currQuestion; ?> / <?php echo $total; ?>
            </p>
            <progress  class="progressbar" value="<?php echo $currQuestion; ?>" max="<?php echo $total; ?>" ></progress>
            
            <div class="question">
                <div class="questiontextwrapper">
                    <h2 class="questiontitle">
                        <?php echo($question->title);?>
                    </h2>
                    <p class="questiontext">
                        <?php echo($question->text);?>
                    </p>
                </div>
            <?php
                    if($question !== NULL)
                    {
            ?>
            
            
                <img class="questionimage" src="data:image/jpeg;base64,<?php echo(base64_encode($image));?>"></img>
            <?php
                    }
            ?>
            </div>
            
            
            <form action="index.php?uid=<?php echo $_SESSION['hid']; ?>&hash=<?php echo $_SESSION['hhash']; ?>" method="post" 
                    <?php 
                    if($currQuestion == $total)
                    { 
                    ?> 
                        onsubmit="return confirm('Dit is de laatste vraag. U zal niets meer kunnen wijzigen na u deze hebt ingediend. Bent u zeker dat u door wil gaan?')" 
                    <?php
                    }
                    ?>
                  >
                
                <p>Hoe ervaart u dit onderdeel?</p>
                <div class="answercontainer" id="primary">
                    <label class="answerboxlabel" onclick="hide()">
                        <input type="radio" name="primaryanswer" value="1" required />
                        <div class="answerbox">
                            <p class="answerboxtext">
                                Verloopt naar wens
                            </p>
                        </div>
                    </label>
                    <label class="answerboxlabel" onclick="hide()">
                        <input type="radio" name="primaryanswer" value="2" required />
                        <div class="answerbox">
                            <p class="answerboxtext">
                                Probleem - niet hinderlijk
                            </p>
                        </div>
                    </label>
                    <label class="answerboxlabel" onclick="show()">
                        <input type="radio" name="primaryanswer" value="3" required />
                        <div class="answerbox">
                            <p class="answerboxtext">
                                Probleem - hinderlijk voor cliënt
                            </p>
                        </div>
                    </label>
                    <label class="answerboxlabel" onclick="show()">
                        <input type="radio" name="primaryanswer" value="4" required />
                        <div class="answerbox">
                            <p class="answerboxtext">
                                Probleem - hinderlijk voor mantelzorger
                            </p>
                        </div>
                    </label>
                    <label class="answerboxlabel" onclick="show()">
                        <input type="radio" name="primaryanswer" value="5" required />
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
                        <input type="radio" name="secondaryanswer" value="1" id="touncheck"/>
                        <div class="answerbox">
                            <p class="answerboxtext">
                                Ja
                            </p>
                        </div>
                    </label>
                    <label class="answerboxlabel visible">
                        <input type="radio" name="secondaryanswer" value="0"  checked="checked" id="tocheck"/>
                        <div class="answerbox">
                            <p class="answerboxtext">
                                Nee
                            </p>
                        </div>
                    </label>
                    
                    <input type="submit" class="submitbutton" value="Volgende vraag">
                </div>
                
            </form>
            <div class="auxcontrolbox">
                <form action="index.php?uid=<?php echo $_SESSION['hid']; ?>&hash=<?php echo $_SESSION['hhash']; ?>" method="post" 
                      onsubmit="return confirm('Bent u zeker dat u opnieuw wilt beginnen? Uw voortgang wordt gewist.')">
                    <input type="radio" name="reset" value="1" class="nodisplay" checked="checked"/>
                    <input type="submit" class="submitbutton auxcontrol lightblue" value="Herstarten">
                </form>

                <form action="index.php?uid=<?php echo $_SESSION['hid']; ?>&hash=<?php echo $_SESSION['hhash']; ?>" method="post">
                    <input type="radio" name="previous" value="1" class="nodisplay" checked="checked"/>
                    <input type="submit" class="submitbutton auxcontrol purple" value="Vorige vraag">
                </form>

                <form action="index.php?uid=<?php echo $_SESSION['hid']; ?>&hash=<?php echo $_SESSION['hhash']; ?>" method="post">
                    <input type="radio" name="instructions" value="1" class="nodisplay" checked="checked"/>
                    <input type="submit" class="submitbutton auxcontrol lightblue" value="Instructies">
                </form>
            </div>
        </div>
                        <?php
                    }
                }  
            }
        ?> 
         
        <script>
            // Hides the help section and sets the secondaryanswer box with
            // value zero to checked, unchecking the box with value one if it
            // is checked.
            function hide(){
                // Run through all elements with class 'visible' and give them
                // class 'hidden' instead
                var toHide = document.getElementsByClassName('visible');
                while(toHide.length > 0)
                    toHide[0].className = toHide[0].className.replace(/(?:^|\s)visible(?!\S)/g , ' hidden');
                
                if(document.getElementById('touncheck') !== null)
                    document.getElementById('touncheck').checked = false;
                if(document.getElementById('tocheck') !== null)
                    document.getElementById('tocheck').checked = true;
            }
            
            // Shows the help section
            function show(){
                // Run through all elements with class 'hidden' and give them
                // class 'visible' instead
                var toShow = document.getElementsByClassName('hidden');
                while(toShow.length > 0)
                    toShow[0].className = toShow[0].className.replace(/(?:^|\s)hidden(?!\S)/g , ' visible');
            }
            
            // Removes the preload class from the body element
            function removePreload(){
                document.body.className = document.body.className.replace(/(?:^|\s)preload(?!\S)/g , ' ');
            }
            
            // Hides the help section and enables transitions by removing the
            // preload class from the body element
            function start(){
                hide();
                // We have to wait a bit to remove the preload class or else 
                // the animation will play on page load.
                setTimeout(removePreload, 10);
            }
        </script>
    </body>
</html>
