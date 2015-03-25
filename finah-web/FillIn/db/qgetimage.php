<?php
    $GETIMAGEQUERY =    
           "SELECT question.image 
            FROM client, form, questionnaire, questionlist, question 
            WHERE client.id = ? 
            AND client.hash = ?  
            AND client.formid = form.id 
            AND form.airid = questionnaire.id 
            AND questionnaire.id = questionlist.airid 
            AND questionlist.enabled = TRUE 
            AND questionlist.qid = question.id 
            AND question.id = ? 
            ;";

?>
