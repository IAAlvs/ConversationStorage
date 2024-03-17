CREATE  KEYSPACE [IF NOT EXISTS] conversations
   WITH REPLICATION = { 
      'class' : 'SimpleStrategy', 'replication_factor' : N } 
   [AND DURABLE_WRITES =  true] ;


CREATE KEYSPACE IF NOT EXISTS isaac_space
WITH replication = {
   'class': 'SimpleStrategy', 
   'replication_factor': 1
};
