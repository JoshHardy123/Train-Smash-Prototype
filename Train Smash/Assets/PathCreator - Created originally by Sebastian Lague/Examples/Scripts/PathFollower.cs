using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PathCreation.Examples
{
    // ORIGINAL SCRIPT CREATED BY SEBASTIAN LAGUE. ALTERED BY ME
    public class PathFollower : MonoBehaviour
    {
        public enum TrainStates
        {
            Accelerating,
            Coasting,
            Braking,
            Stopped,
            RollingBack
        }
        public TrainStates currentState;
        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;
        public bool canMove = false;
        public bool destinationReached { get; set; }
        List<CarriageFollower> allTrainCarriages = new List<CarriageFollower>();

        [Header("Speed")]
        public float currentSpeed = 0;
        public float topSpeed = 5;
       public float timeToTopSpeed = 3f;
        public float decelerateTime = 10f;
       // public float acceleration = 1f;
        //public float coastDeceleration = 0.5f;
        float velocity = 0f;
        float distanceTravelled;

        [Header("Fuel")]
        public int maxFuel;
        public float CurrentFuel;
        public float fuelUsagePerSecond = 1f;
        public ParticleSystem steamFX;

        [Header("Transforming")]
        public float YaxisOffset=0.5f;
        public float zRot =90;
        public float startDist = 5f;
        public float endDist = 20f; //USED ONLY FOR THE SLIDER OF LEVEL PROGRESS

        [Header("Gravity")]
        public float gravity = 9.81f;

        [Header("Braking")]
        public float brakeTime = 0.5f;
        public ParticleSystem[] sparks;
        Transform brakeTarget;
        bool sparksEnabled = false;

        [Header("UserInterface")]
        public Slider levelProgressSlider;
        public Slider fuelSlider;
        private void Awake()
        {
            foreach (var item in GameObject.FindGameObjectsWithTag("Train"))
            {
                allTrainCarriages.Add(item.GetComponent<CarriageFollower>());
            }
            currentState = TrainStates.Stopped;
        }

        void Start() {
            if (pathCreator != null)
            {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
                pathCreator.pathUpdated += OnPathChanged;
            }
            distanceTravelled = startDist;
            CurrentFuel = maxFuel;
            brakeTarget = GameObject.FindGameObjectWithTag("Endpoint").transform;

            if (!levelProgressSlider) levelProgressSlider = GameObject.FindGameObjectWithTag("ProgressSlider").GetComponent<Slider>();
            if(!fuelSlider) fuelSlider = GameObject.FindGameObjectWithTag("FuelSlider").GetComponent<Slider>();
        }

        void Update()
        {
            //INPUT CONDITIONS
            if (Input.GetButton("Fire1") && canMove) currentState = TrainStates.Accelerating;
            else if (canMove) currentState = TrainStates.Coasting;


            //VARIABLE CONDITIONS
            if (!canMove && currentSpeed > 0 && destinationReached)  currentState = TrainStates.Braking; //BRAKING
            if (currentSpeed == 0 && currentState!= TrainStates.Accelerating) currentState = TrainStates.Stopped; //STOPPED
            if (!canMove && currentSpeed > 0 && !destinationReached) currentState = TrainStates.Coasting; //COASTING
            if(currentSpeed < 0 && currentState != TrainStates.Accelerating) currentState = TrainStates.RollingBack;//ROLLING BACKWARDS
            

            //STATE LOGIC
            switch (currentState)
            {
                case TrainStates.Accelerating:
                     currentSpeed = Mathf.SmoothDamp(currentSpeed, topSpeed, ref velocity, timeToTopSpeed);
                    if (!steamFX.isPlaying) steamFX.Play();
                    //if(currentSpeed<topSpeed)currentSpeed += acceleration * Time.deltaTime;
                    UseFuel();
                    break;
                case TrainStates.Coasting:
                    currentSpeed = Mathf.SmoothDamp(currentSpeed, 0, ref velocity, decelerateTime);
                    if (steamFX.isPlaying) steamFX.Stop();
                    //if (currentSpeed > 0) currentSpeed -= coastDeceleration * Time.deltaTime;
                    //else currentSpeed += coastDeceleration * Time.deltaTime;
                    break;
                case TrainStates.Braking:
                    currentSpeed = Mathf.SmoothDamp(currentSpeed, 0, ref velocity, brakeTime);
                    if(!sparksEnabled)EnableDisableSparks(true);
                    if (currentSpeed <= 0.5f) currentSpeed = 0;
                    if (steamFX.isPlaying) steamFX.Stop();

                    break;
                case TrainStates.Stopped:
                    if(sparksEnabled)EnableDisableSparks(false);
                    if (steamFX.isPlaying) steamFX.Stop();

                    break;
                case TrainStates.RollingBack:
                    currentSpeed = Mathf.SmoothDamp(currentSpeed, 0, ref velocity, decelerateTime);
                    if (steamFX.isPlaying) steamFX.Stop();

                    break;
                default:
                    break;
            }

            Gravity(currentSpeed); 

            if (pathCreator != null) // Updating position on Track
            {
                distanceTravelled += currentSpeed * Time.deltaTime;
                distanceTravelled = Mathf.Clamp(distanceTravelled,startDist,pathCreator.path.length);
                SpeedCheck();
                transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);

                for (int i = 0; i < allTrainCarriages.Count; i++)
                {
                    allTrainCarriages[i].transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled-allTrainCarriages[i].distBehindFrontOfTrain, endOfPathInstruction) + new Vector3(0, YaxisOffset, 0); 
                    Quaternion rot = pathCreator.path.GetRotationAtDistance(distanceTravelled - allTrainCarriages[i].distBehindFrontOfTrain, endOfPathInstruction);
                    rot.eulerAngles += new Vector3(0, 0, zRot);
                    allTrainCarriages[i].transform.rotation = rot;
                }

               // if (distanceTravelled + endDist >= pathCreator.path.length) ToggleMovement(false);

                transform.position += new Vector3(0,YaxisOffset,0);

                Quaternion newRot = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
                newRot.eulerAngles += new Vector3(0,0,zRot);
                transform.rotation = newRot;
            }

                UpdateUI();
           
        }
        void EnableDisableSparks(bool enable)
        {
            sparksEnabled = enable;
            for (int i = 0; i < sparks.Length; i++)
            {
               if(enable) sparks[i].Play();
               else sparks[i].Stop();
            }
        }

        void SpeedCheck()
        {
            if (distanceTravelled <= startDist && currentSpeed < 0) currentSpeed = 0;

            //else if (distanceTravelled >= pathCreator.path.length - endDist && currentSpeed > 0) currentSpeed = 0;
        }

        void UpdateUI()
        {
            levelProgressSlider.value = (distanceTravelled-startDist) / (pathCreator.path.length-endDist);
            fuelSlider.value = CurrentFuel/maxFuel;
        }
        void UseFuel()
        {
            CurrentFuel -= fuelUsagePerSecond * Time.deltaTime;
            if (CurrentFuel <= 0) ToggleMovement(false);
        }
        public void ToggleMovement(bool canMove)
        {
            this.canMove = canMove;   
        }

        public void SetSpeed(float speed)
        {
            currentSpeed = speed;
        }
        public void Gravity(float currentspeed)
        {
            float angle = Vector3.Angle(Vector3.down,transform.forward);
            if(angle < 85 || angle > 95 )
            {
                currentSpeed += gravity * Time.deltaTime * (90-angle);
                //pull forward by gravity * (1-(angle/90))
            }
            //else if(angle > 100)
            //{
            //    currentSpeed += gravity * Time.deltaTime * (1 + ((100-angle) / 100));
            //    //pull backwards by gravity * (1-(angle/90))
            //}
            //Debug.Log(angle);
           // currentSpeed = Mathf.Clamp(currentSpeed,-topSpeed*1.3f,topSpeed*1.3f);
        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        void OnPathChanged() {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }
    }
}