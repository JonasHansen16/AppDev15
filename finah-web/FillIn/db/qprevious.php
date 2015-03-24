<?php
// CURRENTLY THIS QUERY OPERATES LIKE A SIMPLE UPDATE
// TODO: ADD SECURITY AND SAFETY CHECKS
// THIS WILL HAVE TO BE DONE WHILE WRITING THE API

$PREVIOUSQUERY = 
       "UPDATE answer 
        SET final = FALSE 
        WHERE clientid = ? 
        AND qid = 
        ( 
            SELECT mqid 
            FROM 
		( 
                    SELECT MAX(qid) as mqid 
                    FROM answer 
                    WHERE clientid = ? 
                ) as temp 
        ) 
        ;";
?>
