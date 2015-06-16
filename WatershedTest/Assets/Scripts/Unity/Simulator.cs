using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/// <summary>
/// The Simulator class will host single or multiple model runs.
/// The simulator class will "run" models based on time steps provided by the user.
/// </summary>
public class Simulator
{
    public float TimeStep;
    public DateTime start;
    public DateTime end;
    // Default one hour
    public TimeSpan TimeDelta;
    List<ModelRun> ModelRuns = new List<ModelRun>();

    // Storing byte data and float data
    List<byte> Texture = new List<byte>();
    List<float[,]> RawData = new List<float[,]>();

    // This is used for determining what in the model run we are focusing on.
    public string ModelVar = "";

    // How many datasets do we have?
    int NumberOfDatasets = 0;

    // Number of datasets that are loaded into
    int Loaded = 0;

    // Threshold for how many we should have in ram
    int MaxLoaded = 1000;



    public Simulator()
    {
        start = new DateTime(1997, 1, 1);
        end = new DateTime(1998, 1, 1);
        TimeDelta = new TimeSpan(1, 0, 0);
        DataRecord current = null;
        if(current != null && current.texture != null)
        {
            // Print something out here...
        }
    }

    public void SetStartDate(DateTime Time)
    {
        start = Time;
    }

    public void SetEndDate(DateTime Time)
    {
        end = Time;
    }

    public void SetModelRun(ModelRun mr)
    {
        ModelRuns.Add(mr);
        ModelVar = mr.Query()[0].variableName;
    }

    // A test simulation function for debugging purposes.
    public void Simulation(float dt)
    {
        currentTimeFrame = 0;
        Logger.WriteLine("RUNNING SIMULATION");
        
        TimeStep = dt;
        DateTime CurrentTime = start;
        Logger.WriteLine(CurrentTime.ToString());
        Logger.WriteLine(end.ToString());

        int status = 0;
        while(CurrentTime  < end )
        {
            if(status != -1)
            {
                currentTimeFrame = status;
            }
            // Step through current data...
            if(currentTimeFrame == -1)
            {
                Logger.WriteLine(currentTimeFrame.ToString());
                status = ModelRuns[0].Update(ModelVar, currentTimeFrame, CurrentTime);
                CurrentTime = CurrentTime.AddTicks((long)(dt * TimeDelta.Ticks));
                continue;
            } 
            Logger.WriteLine(CurrentTime.ToString() + " " + currentTimeFrame);
            
            status = ModelRuns[0].Update(ModelVar, currentTimeFrame, CurrentTime);

            // Download Function.... --- Maybe with a callback that sends the data to the proper place...

            CurrentTime = CurrentTime.AddTicks((long)(dt * TimeDelta.Ticks));
        }
    }

    // A step function for custom timesteps
    public void Step(float timestep)
    {

    }

    // This is the previous record that was used.
    int currentTimeFrame = 0;

    // A setter function for setting the model runs.
    public void SetModelRuns(List<ModelRun> Models)
    {
        ModelRuns.Clear();
        ModelRuns.AddRange(ModelRuns);
    }
    

    /// <summary>
    /// This FetchData function will get data from the model run classes.
    /// </summary>
    private void FetchData(float range=0.0f)
    {
        // Time to build he fetcher .......
        if(range > 1 )
        {
            range = 1;
        }
        else if(range < 0)
        {
            range = 0;
        }
        // TODO
        // Fetch data

        // check cache for object
    }

    private void FetchData(DateTime date)
    {
        // Fetch the dataset that closely matches this date

    }

    /// <summary>
    /// Pulls all of the data for a all model runs.
    /// </summary>
    public void FetchAll(DataRecordSetter SendToSpooler,string service="wms",SystemParameters parameters=null)
    {
        if(service=="wms" && parameters == null)
        {
            parameters = new SystemParameters();
            parameters.width = 100;
            parameters.height = 100;
            parameters.format = "png";
        }

        // Fetch all data related to all model runs -- 
        foreach(var i in ModelRuns)
        {
            i.FetchAll(ModelVar,SendToSpooler,service,parameters);
        }
    }

}

