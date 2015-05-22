<?php
    // CURRENTLY THIS QUERY OPERATES LIKE A SIMPLE INSERT
    // TODO: ADD SECURITY AND SAFETY CHECKS
    // THIS WILL HAVE TO BE DONE WHILE WRITING THE API

    $INSERTANSWERQUERY = 
           "INSERT INTO answer (clientid, qid, score, help, final) 
            VALUES (?, ?, ?, ?, TRUE) 
            ;";
?>
