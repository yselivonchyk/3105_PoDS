
public class TokenRing 
{
	private static TokenRing instance=new TokenRing(); //just for synchronization locking
	private static boolean tokenExistance = false;
	private static boolean haveToken = false;
	private static boolean wantToken = false;
	
	protected static void initializeTokenRing() 
	{
		tokenExistance = true;
		haveToken = false;
		wantToken = false;
	}
	
	protected static void startTokenRingAlgorithm()
	{
		 if(!tokenExistance)
         {
             System.out.println("Token was not initialized. Can't start!");
             System.exit(0);;
         }
         System.out.println("### Token Ring running ###");
         haveToken = true;
	}
	
	protected static void stopTokenRingAlgorithm()
	{
		if (!tokenExistance)
        {
            System.out.println("The algorithm didn't start. Wrong!");
            System.exit(0);
        }
        System.out.println("### Token Ring stopped ###");
        tokenExistance = false;
        haveToken = false;
        wantToken = false;
	}
	
	protected static void waitForToken()
	{
		System.out.println("### Waiting for receiving token ###");
        if (!tokenExistance)
        {
            System.out.println("The algorithm didn't start. Wrong!");
            System.exit(0);
        }
        wantToken = true;
        Boolean flag = false;
        while (!flag)
            synchronized (instance)
            {
                flag = haveToken;
            }		
	}
	
	protected static void sendToken() 
	{
		if (!tokenExistance)
        {
            System.out.println("The algorithm has been stopped. Stop sending token.");
            return;
        }
        haveToken = false;
        wantToken = false;
        new TokenSender(TokenRingClient.nextHostOnRing());
	}
	
	public static int takeTheToken(int ack)
	{
		if (!tokenExistance)
        {
            System.out.println("The algorithm has been stop. Don't take the token.");
            return 0;
        }
        if (wantToken)
        {
            synchronized (instance)
            {
                haveToken = true;
            }

            System.out.println("### Receive Token ###");
        }
        else
        {
            sendToken();
        }
        return ack + 1;
    }
}


