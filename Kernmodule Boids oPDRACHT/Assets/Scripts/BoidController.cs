using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    public int swarmIndex { get; set; }
    public float noClumpingRadius { get; set; }
    public float localAreaRadius { get; set; }
    public float speed { get; set; }
    public float steeringSpeed { get; set; }



    public void SimulateMovement(List<BoidController> other, float time)
    {
        Vector3 steering = Vector3.zero;

        Vector3 separationDirection = Vector3.zero;
        float separationCount = 0;
        Vector3 alignmentDirection = Vector3.zero;
        float alignmentCount = 0;
        Vector3 cohesionDirection = Vector3.zero;
        float cohesionCount = 0;

        var leaderBoid = other[0];
        float leaderAngle = 180f;

        foreach(BoidController boid in other)
        {
            if(boid == this)
            {
                continue;
            }

            var distance = Vector3.Distance(boid.transform.position, this.transform.position);

            if(distance < noClumpingRadius)
            {
                separationDirection += boid.transform.position - transform.position;
                separationCount++;
            }

            if(distance < localAreaRadius &&  boid.swarmIndex == this.swarmIndex)
            {
                alignmentDirection += boid.transform.forward;
                alignmentCount++;

                cohesionDirection += boid.transform.position - transform.position;
                cohesionCount++;

                var angle = Vector3.Angle(boid.transform.position - transform.position, transform.forward);
                if(angle < leaderAngle && angle < 90f)
                {
                    leaderBoid = boid;
                    leaderAngle = angle;
                }
            }
        }

        if(separationCount > 0)
        {
            separationDirection /= separationCount;
        }

        separationDirection = -separationDirection;

        if (alignmentCount > 0)
            alignmentDirection /= alignmentCount;

        if (cohesionCount > 0)
            cohesionDirection /= cohesionCount;

        //get direction to center of mass
        cohesionDirection -= transform.position;

        //weighted rules
        steering += separationDirection.normalized;
        steering += alignmentDirection.normalized;
        steering += cohesionDirection.normalized;

        //local leader
        if (leaderBoid != null)
            steering += (leaderBoid.transform.position - transform.position).normalized;

        //obstacle avoidance
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, localAreaRadius, LayerMask.GetMask("Default")))
            steering = ((hitInfo.point + hitInfo.normal) - transform.position).normalized;

        //apply steering
        if (steering != Vector3.zero)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(steering), steeringSpeed * time);

        //move 
        transform.position += transform.TransformDirection(new Vector3(0, 0, speed)) * time;

    }
}
