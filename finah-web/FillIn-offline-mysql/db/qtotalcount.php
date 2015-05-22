<?php
$TOTALCOUNTQUERY = 
       "SELECT COUNT(questionlist.qid) AS amount 
        FROM client, form, questionnaire, questionlist 
        WHERE client.id = ? 
        AND client.hash = ? 
        AND client.formid = form.id 
        AND form.airid = questionnaire.id 
        AND questionnaire.id = questionlist.airid 
        AND questionlist.enabled = TRUE 
        ;";
?>
