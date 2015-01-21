package uni_bonn.pds;

import java.io.IOException;
import java.util.logging.FileHandler;
import java.util.logging.Logger;
import java.util.logging.SimpleFormatter;

public class Log {

	public static Logger logger;
	static {
		logger = Logger.getLogger("MyLog");
		FileHandler fh;
		try {
			fh = new FileHandler("Calculations.log",false); 
			logger.addHandler(fh);
			SimpleFormatter formatter = new SimpleFormatter();
			fh.setFormatter(formatter);
		} catch (SecurityException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}

	}
}