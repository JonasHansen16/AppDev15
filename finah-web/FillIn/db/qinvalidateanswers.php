<?php
// CURRENTLY THIS QUERY OPERATES LIKE A SIMPLE UPDATE
// TODO: ADD SECURITY AND SAFETY CHECKS
// THIS WILL HAVE TO BE DONE WHILE WRITING THE API

$INVALIDATEANSWERSQUERY = 
       "UPDATE answer
        SET final = FALSE
        WHERE clientid = ? AND qid = ?
        ;";
?>
