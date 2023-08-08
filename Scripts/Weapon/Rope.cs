using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class Rope : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public EdgeCollider2D EdgeCollider;
    public Attachment attachment;
    
    public int JointCount = 25;
    public int ConstraintLoop = 15;
    public float JointLength = 0.04f;
    public float RophWidth = 0.01f;

    public Vector2 Weight = new Vector2(0.0f, -4.5f);
    Vector2[] colliderPositions;

    Vector3[] jointPositions;

    //Test
    public float RophMoveLength = 1.0f;
    Vector3[] stopPos;

    [Space(10f)]
    public Transform StartTransform;

    private List<Joint> Joints = new List<Joint>();

    private void Reset()
    {
        TryGetComponent(out lineRenderer);
        TryGetComponent(out EdgeCollider); 
        TryGetComponent(out attachment);
    }

    void Start()
    {
        colliderPositions = new Vector2[Joints.Count];
        jointPositions = new Vector3[Joints.Count];
        stopPos = new Vector3[Joints.Count];
    }

    private void Awake()
    {
        //colliderPositions = new Vector2[Joints.Count];

        Vector2 jointPos = StartTransform.position;
        for(int i = 0; i < JointCount; i++)
        {
            jointPos.y -= JointLength;
            Joints.Add(new Joint(jointPos));

            //colliderPositions[i] = jointPos;
            //colliderPositions[i].y -= JointLength;
        }
    }

    private void FixedUpdate()
    {
        if(!attachment.bHit)
        {
            UpdateJoints();
    
            for(int i = 0; i < ConstraintLoop; i++)
            {
                ApplyConstraint();
                AdjustCollision();
            }
        }
    
        DrawRope();    
    }

    private void DrawRope()
    {
        lineRenderer.startWidth = RophWidth;
        lineRenderer.endWidth = RophWidth;

        //Vector3[] jointPositions = new Vector3[Joints.Count]; //Vector3

        for (int i = 0; i < Joints.Count; i++)
        {
            jointPositions[i] = Joints[i].currentPos;
            //TODO : 로프 시작위치 조정
            jointPositions[i].x -= 0.1f;
            jointPositions[i].y -= 0.7f;

            colliderPositions[i].y = Joints[i].currentPos.y;
            //colliderPositions[i] = JointPositions[i];
        }
        lineRenderer.positionCount = jointPositions.Length;
        lineRenderer.SetPositions(jointPositions);

        if(EdgeCollider)
        {
            EdgeCollider.edgeRadius = RophWidth;
            EdgeCollider.points = colliderPositions;
        }

        if(attachment)
        {
            attachment.transform.position = jointPositions[Joints.Count - 1];
        }
    }

    private void UpdateJoints()
    {
        for (int i = 0; i < Joints.Count; i++)
        {
            Joints[i].velocity = Joints[i].currentPos - Joints[i].prevPos;
            Joints[i].prevPos = Joints[i].currentPos;

            if(attachment)
                Joints[i].currentPos += (Weight + attachment.GetWeight()) * Time.fixedDeltaTime * Time.fixedDeltaTime;
            else
                Joints[i].currentPos += Weight * Time.fixedDeltaTime * Time.fixedDeltaTime;

            Joints[i].currentPos += Joints[i].velocity;
        }
    }

    private void ApplyConstraint()
    {
        Joints[0].currentPos = StartTransform.position;
        colliderPositions[0] = StartTransform.position;

        for (int i = 0; i < Joints.Count - 1; i++)
        {
            float distance = (Joints[i].currentPos - Joints[i + 1].currentPos).magnitude;
            float offset = JointLength - distance;
            Vector2 dir = (Joints[i + 1].currentPos - Joints[i].currentPos).normalized;

            Vector2 movement = dir * offset;
            
            if (i == 0)
            {
                Joints[i + 1].currentPos += movement;
            }
            else
            {
                Joints[i].currentPos -= movement * 0.5f;
                Joints[i + 1].currentPos += movement * 0.5f;
            }
        }

    }

    private void AdjustCollision()
    {
        for(int i = 0; i < Joints.Count; i++)
        {
            Vector2 dir = Joints[i].currentPos - Joints[i].prevPos;
            //RaycastHit2D hit = Physics2D.CircleCast(Joints[i].currentPos, RophWidth * 0.5f, dir.normalized, 0.0f, (-1) - (1 << 12));
            
            //Attachment, Projectile, Weapon_Rotate, Weapon_Attach
            //RaycastHit2D hit = Physics2D.CircleCast(Joints[i].currentPos, RophWidth * 0.5f, dir.normalized, 0.0f, (-1) - ((1 << 7) + (1 << 12)) + (1 << 13) + (1 << 14) + (1 << 15) + (1 << 16));

            //if (hit)
            //{
            //    Joints[i].currentPos = hit.point + hit.normal * RophWidth * 0.5f;
            //    Joints[i].prevPos = Joints[i].currentPos;
            //}
        }
    }

    public class Joint
    {
        public Vector2 prevPos;
        public Vector2 currentPos;
        public Vector2 velocity;

        public Joint(Vector2 pos)
        {
            prevPos = pos;
            currentPos = pos;
            velocity = Vector2.zero;
        }
    }

}
