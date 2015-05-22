<?php
    /// BEGIN SESSION \\\
    session_start();
?>

<html>
    <head>
        <meta charset="UTF-8">
        <title>FillIn vragenlijst</title>
        <link rel="stylesheet" href="../layout/main.css">
        <link rel="stylesheet" href="../layout/light.css">
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
         * Sends a request to a REST API and returns the result.
         * @param string $url The url to consume.
         * @param $params The parameters to pass.
         * @param $verb POST or GET.
         * @param $format json or xml.
         * @return The API's response.
         * @throws Exception When an error occurs.
         */
        function rest_helper($url, $params = null, $verb = 'POST', $format = 'json')
        {
            $cparams = array(
                'http' => array(
                'method' => $verb,
                'ignore_errors' => false
                )
            );
            if ($params !== null) {
                $params = http_build_query($params);
                $cparams['http']['content'] = $params;
                $url .= '?' . $params;
            }

            $context = stream_context_create($cparams);
            $fp = fopen($url, 'rb', false, $context);
            if (!$fp) {
                $res = false;
            } else {
                $res = stream_get_contents($fp);
            }

            if ($res === false) {
                throw new Exception("$verb $url failed: $php_errormsg");
            }

            switch ($format) {
                case 'json':
                    $r = json_decode($res);
                    if ($r === null) {
                        throw new Exception("failed to decode $res as json");
                    }
                    return $r;

                case 'xml':
                    $r = simplexml_load_string($res);
                if ($r === null) {
                    throw new Exception("failed to decode $res as xml");
                }
                return $r;
            }
            return $res;
        }
        
        /**
         * Checks whether or not the client specified by the id and hash
         * parameters exists or not. If the client exists, $_SESSION['hid']
         * and $_SESSION['hhash'] will be set for the duration of the session. 
         * The current value of $_SESSION['hid'] and $_SESSION['hhash'] will be lost 
         * upon calling this function.
         * @param $id The id of the client.
         * @param $hash The hash of the client.
         * @return True if the client exists; false otherwise.
         */
        function checkHash($id, $hash){
            require '../db/db.php';
            
            $url = $APIHOST . 'api/client/Exists/';
            try{
                $result = rest_helper($url, array('id' => $id, 'hash' => $hash));
            } catch (Exception $ex){
                return false;
            }
            if($result === FALSE)            
                return false;
            
            $_SESSION['hid'] = $id;
            $_SESSION['hhash'] = $hash;
            return true;
        }

        /**
         * Sets the variables $GLOBALS['done'] and $GLOBALS['start'] if the 
         * client is done with his questionnaire or if he is at the start of it,
         * respectively. Will set both of these to TRUE if an error occurs.
         * @param $id The id of the client.
         * @param $hash The hash of the client.
         */
        function getStartOrDone($id, $hash){
            require '../db/db.php';
            
            $starturl = $APIHOST . 'api/client/Start/';
            $doneurl = $APIHOST . 'api/client/Done/';
            
            try{
                $start = rest_helper($starturl, array('id' => $id, 'hash' => $hash));
                $done = rest_helper($doneurl, array('id' => $id, 'hash' => $hash));
            } catch(Exception $ex){
                $GLOBALS['start'] = true;
                $GLOBALS['done'] = true;
                return;
            }
            
            $GLOBALS['start'] = $start;
            $GLOBALS['done'] = $done;
        }
        
        /**
         * Returns the next unanswered question of the given client.
         * @param $id The id of the client.
         * @param $hash The hash of the client.
         * @return question An object containing the title, the text and the id 
         * of the first unanswered question. Will be NULL if there are no more
         * questions.
         */
        function getNextUnansweredQuestion($id, $hash){
            require '../db/db.php';
            
            $url = $APIHOST . 'api/question/Next/';
            
            try{
                $result = rest_helper($url, array('id' => $id, 'hash' => $hash));
            }
            catch(Exception $ex){
                return null;
            }
            
            $output = new question();
            $output->id = $result->Id;
            $output->title = $result->Title;
            $output->text = $result->Text;
            
            return $output;
        }
        
        /**
         * Returns the intro of the questionnaire of the given client.
         * @param $id The id of the client.
         * @param $hash The hash of the client.
         * @return introduction An object containing the title and the 
         * introduction text of the questionnaire. Will be NULL if the client
         * does not exist.
         */
        function getIntro($id, $hash){
            require '../db/db.php';
            
            $url = $APIHOST . 'api/questionnaire/Intro/';
            
            try{
                $result = rest_helper($url, array('id' => $id, 'hash' => $hash));
            }
            catch(Exception $ex){
                return null;
            }
            
            $output = new introduction();
            $output->title = $result->Title;
            $output->text = $result->Text;
            
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
         */
        function setStart($id, $hash){
            require '../db/db.php';
            
            $url = $APIHOST . 'api/client/SetStart/';
            
            try{
                $result = rest_helper($url, array('id' => $id, 'hash' => $hash));
            }
            catch(Exception $ex){
                return;
            }
        }
        
        /**
         * Sets the 'start' value of the client with id $id and hash $hash to
         * false.
         * @param $id The id of the client.
         * @param $hash The hash of the client.
         */
        function unsetStart($id, $hash){
            require '../db/db.php';
            
            $url = $APIHOST . 'api/client/UnSetStart/';
            
            try{
                $result = rest_helper($url, array('id' => $id, 'hash' => $hash));
            }
            catch(Exception $ex){
                return;
            }
        }
        
        /**
         * Sets the 'done' value of the client with id $id and hash $hash to
         * true.
         * @param $id The id of the client.
         * @param $hash The hash of the client.
         */
        function setDone($id, $hash){
            require '../db/db.php';
            
            $url = $APIHOST . 'api/client/Done/';
            
            try{
                $result = rest_helper($url, array('id' => $id, 'hash' => $hash));
            }
            catch(Exception $ex){
                return;
            }
        }
        
        /**
         * Sets the 'done' value of the client with id $id and hash $hash to
         * false.
         * @param $id The id of the client.
         * @param $hash The hash of the client.
         */
        function unsetDone($id, $hash){
            require '../db/db.php';
            
            $url = $APIHOST . 'api/client/UnSetDone/';
            
            try{
                $result = rest_helper($url, array('id' => $id, 'hash' => $hash));
            }
            catch(Exception $ex){
                return;
            }
        }
        
        /**
         * Restarts the client's questionnaire by invalidating all his answers.
         * @param $id The id of the client.
         * @param $hash The hash of the client.
         */
        function resetQuestionnaire($id, $hash){
            require '../db/db.php';
            
            $url = $APIHOST . 'api/answer/Reset/';
            
            try{
                $result = rest_helper($url, array('id' => $id, 'hash' => $hash));
            }
            catch(Exception $ex){
                return;
            }
        }
        
        /**
         * Returns the client to his previous question by invalidating his
         * latest answer.
         * @param type $id The id of the client.
         * @param type $hash The hash of the client.
         */
        function previousQuestion($id, $hash){
            require '../db/db.php';
            
            $url = $APIHOST . 'api/question/PreviousQuestion/';
            
            try{
                $result = rest_helper($url, array('id' => $id, 'hash' => $hash));
            }
            catch(Exception $ex){
                return;
            }
        }
        
        /**
         * Inserts an answer into the database, invalidating all other answers
         * for the same question by the same user by setting their 'final' value
         * to false. Note that $help will be set to false if $score is lower
         * than three, regardless of its actual value.
         * @param type $id The id of the client.
         * @param type $hash The hash of the client.
         * @param type $qid The id of the question to insert.
         * @param type $score The score of the question to insert.
         * @param type $help Whether or not the client has requested aid.
         */
        function insertAnswer($id, $hash, $qid, $score, $help){
            require '../db/db.php';
            
            $url = $APIHOST . 'api/answer/Insert/';
            
            // If score is lower than three, set help to false.
            if($score < 3)
                $help = 0;
            
            try{
                $result = rest_helper($url, array('id' => $id, 'hash' => $hash, 'qid' => $qid, 'score' => $score, 'help' => $help));
            }
            catch(Exception $ex){
                return;
            }
        } 
        
        /**
         * Returns the total amount of questions in the questionnaire of the 
         * given client.
         * @param $id The id of the client.
         * @param $hash The hash of the client.
         * @return int The total amount of questions in the questionnaire of
         * the given client. Will be 0 if the client does not exist.
         */
        function totalCount($id, $hash){
            require '../db/db.php';
            
            $url = $APIHOST . 'api/questionnaire/Count/';
            
            try{
                $result = rest_helper($url, array('id' => $id, 'hash' => $hash));
            } catch (Exception $ex){
                return 0;
            }
            
            if($result < 0)
                return 0;
            
            return $result;
        }
        
        /**
         * Returns the total amount of different valid answers the client has 
         * already given.
         * @param $id The id of the client.
         * @param $hash The hash of the client.
         * @return int The total amount of different valid answers the client has 
         * already given. Will be 0 if the client does not exist.
         */
        function answerCount($id, $hash){
            require '../db/db.php';
            
            $url = $APIHOST . 'api/answer/Count/';
            
            try{
                $result = rest_helper($url, array('id' => $id, 'hash' => $hash));
            } catch (Exception $ex){
                return 0;
            }
            
            if($result < 0)
                return 0;
            
            return $result;
        }
        
        /**
         * Returns the image belonging to a question.
         * @param $id The id of the client.
         * @param $hash The hash of the client.
         * @param $qid The id of the question.
         * @return int The image of the question, or null if no such question
         * exists or if the user does not have access to that question.
         */
        function getImage($id, $hash, $qid){
            require '../db/db.php';
            
            $url = $APIHOST . 'api/question/Image/';
            
            try{
                $result = rest_helper($url, array('id' => $id, 'hash' => $hash, 'qid' => $qid));
            } catch (Exception $ex){
                return null;
            }
          
            return $result;
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
        // Suppress notices
        error_reporting(E_ALL ^ E_NOTICE);
        // We perform a random query to assertain database connectivity
        $testurl = $APIHOST . 'api/client/Exists/';
        $db = true;
        try{
            $result = rest_helper($testurl, array('id' => 1, 'hash' => 'XCD'));
        } catch (Exception $ex) {
            $db = false;
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
        else if(checkHash($_GET['uid'], $_GET['hash']))
            $pass = true;
        // If the hash lookup fails, we are not authorized to access the questions.
        else
            $pass = false;
        
        // If we are authorized, and we have an answer to submit and that answer is valid
        if($pass && isset($_SESSION['hqid']) && isset($_POST['primaryanswer']) && isset($_POST['secondaryanswer']) && validateAnswerInput($_POST['primaryanswer'], $_POST['secondaryanswer']))
            // We submit the answer to the database
            insertAnswer($_SESSION['hid'], $_SESSION['hhash'], $_SESSION['hqid'], $_POST['primaryanswer'], $_POST['secondaryanswer']); 
        
        // If we are authorized, and we need to start the questionnaire
        if($pass && isset($_POST['started']))
            // We do so by unsetting start
            unsetStart($_SESSION['hid'], $_SESSION['hhash']);
        
        // If we are authorized, and we need to go back to the instructions
        if($pass && isset($_POST['instructions']))
            // We do so by setting start
            setStart($_SESSION['hid'], $_SESSION['hhash']);
        
        // If we are authorized, and we need to restart the questionnaire
        if($pass && isset($_POST['reset']))
            // We do so
            resetQuestionnaire($_SESSION['hid'], $_SESSION['hhash']);
        
        // If we are authoriwed, and we need to return to the previous question
        if($pass && isset($_POST['previous']))
            // We do so
            previousQuestion($_SESSION['hid'], $_SESSION['hhash']);
            
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
                        ernah
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
                getStartOrDone($_SESSION['hid'], $_SESSION['hhash']);
                
                // If we are done, we display an appropriate message.
                if($GLOBALS['done']){
                    printDone();
                }
                // Same for if we are beginning.
                else if($GLOBALS['start']){
                    $intro = getIntro($_SESSION['hid'], $_SESSION['hhash']);
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
                        antwoord zal een blauwe omkadering krijgen. Wees gerust,
                        indien u per ongeluk op het verkeerde antwoord klikt, kan u
                        dit nog altijd veranderen door simpelweg op een ander antwoord
                        te klikken.
                    </p>
                    <figure class="startfigure">
                        <figcaption class="startcaptionleft">Voorbeeld</figcaption>
                        <figcaption class="startcaption">Voorbeeld</figcaption>
                        <img src="../res/answerexample.png" alt="antwoordvoorbeeld" class="startimage">
                        <figcaption class="startcaption">Voorbeeld</figcaption>
                        <figcaption class="startcaptionleft">Voorbeeld</figcaption>
                    </figure>
                    <p class="starttext">
                        Indien u aangeeft dat iets hinderlijk is voor u of uw 
                        mantelzorger, dan krijgt u ook de mogelijkheid om te vragen dat
                        hier aan gewerkt moet worden. U kan dit aangeven net zoals u een
                        vraag beantwoordt. Standaard staat dit op 'Nee'.
                    </p>
                    <figure class="startfigure">
                        <figcaption class="startcaptionleft">Voorbeeld</figcaption>
                        <figcaption class="startcaption">Voorbeeld</figcaption>
                        <img src="../res/helpexample.png" alt="antwoordvoorbeeld" class="startimage">
                        <figcaption class="startcaption">Voorbeeld</figcaption>
                        <figcaption class="startcaptionleft">Voorbeeld</figcaption>
                    </figure>
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
                    $question = getNextUnansweredQuestion($_SESSION['hid'], $_SESSION['hhash']);
                    // If we didn't get a new question, that means we're done.
                    // We update the database and display an appropriate message.
                    if(is_null($question)){
                        setDone($_SESSION['hid'], $_SESSION['hhash']);
                        printDone();
                    }
                    // Else we set our hidden question id, our current and total
                    // question counts, and then finally display the question
                    else{
                        $_SESSION['hqid'] = $question->id;
                        $currQuestion = answerCount($_SESSION['hid'], $_SESSION['hhash']) + 1;
                        $total = totalCount($_SESSION['hid'], $_SESSION['hhash']);
                        $image = getImage($_SESSION['hid'], $_SESSION['hhash'], $_SESSION['hqid']);
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
                    if($image !== NULL)
                    {
            ?>
            
            
                <img class="questionimage" src="data:image/jpeg;base64,<?php echo $image;?>"></img>
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
