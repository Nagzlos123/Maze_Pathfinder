using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Procedural;
using UnityEngine.EventSystems;
using GameAI.PathFinding;

public class MazePathfinder : MonoBehaviour
{
    public Transform mDestination;
    public NPC mNpc;
    public MazeGenerator mMazeGenerator;
    //public GameObject findPathButton;
    // The start and the goal cells.
    private Cell mStart;
    private Cell mGoal;

    LineRenderer mPathViz;
    AStarPathFinder<Vector2Int> mPathFinder = new AStarPathFinder<Vector2Int>();
    // Start is called before the first frame update
    void Start()
    {
        mDestination.gameObject.SetActive(false);
        mNpc.gameObject.SetActive(false);
        //findPathButton.gameObject.SetActive(false);
        // wait for maze generation to complete.
        StartCoroutine(Coroutine_WaitForMazeGeneration());
    }

    IEnumerator Coroutine_WaitForMazeGeneration()
    {
        while (!mMazeGenerator.MazeGenerationCompleted)
            yield return null;
        // set the start cell to cell 0, 0.
        mStart = mMazeGenerator.myMazeCells[0, 0].Cell;
        mGoal = mStart;
        CheakNull();
        // maze generation completed.
        // Set the NPC to cell 0, 0
        mNpc.transform.position = mMazeGenerator.myMazeCells[0, 0].transform.position;
        mDestination.transform.position = mMazeGenerator.myMazeCells[mMazeGenerator.cols - 1, mMazeGenerator.rows - 1].transform.position;
        mNpc.gameObject.SetActive(true);
        
        //findPathButton.gameObject.SetActive(true);
        mNpc.Init();

        // create the line renderer to show the path.
        // We create a line renderer to show the path.
        LineRenderer lineRenderer = mNpc.gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.startColor = Color.magenta;
        lineRenderer.endColor = Color.magenta;

        mPathViz = lineRenderer;

        // set the pathfinder cost functions.
        mPathFinder.HeuristicCost = GetManhattanCost;
        mPathFinder.NodeTraversalCost = GetEuclideanCost;
    
    }

    public float GetManhattanCost(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    public float GetEuclideanCost(Vector2Int a,Vector2Int b)
    {
        return GetCostBetweenTwoCells(a, b);
    }

    public float GetCostBetweenTwoCells(Vector2Int a, Vector2Int b)
    {
        return (mMazeGenerator.myMazeCells[a.x, a.y].transform.position -
          mMazeGenerator.myMazeCells[b.x, b.y].transform.position).magnitude;
    }

   

    public void OnPathFound()
    {
        PathFinder<Vector2Int>.PathFinderNode node = null;
        node = mPathFinder.CurrentNode;
        List<Vector3> reverse_positions = new List<Vector3>();
        while (node != null)
        {
            Vector3 pos = mMazeGenerator.myMazeCells[node.Location.Value.x, node.Location.Value.y].transform.position;
            reverse_positions.Add(pos);
            node = node.Parent;
        }
        LineRenderer lineRenderer = mPathViz;
        lineRenderer.positionCount = reverse_positions.Count;
        for (int i = reverse_positions.Count - 1; i >= 0; i--)
        {
            Vector3 position = reverse_positions[i];
            mNpc.AddWayPoint(new Vector2(position.x, position.y));
            lineRenderer.SetPosition(i, new Vector3(position.x, position.y, -2.0f));
        }
        // changing the start cell for the pathfinder to current goal.
        mStart = mGoal;
    }

    // Update is called once per frame
    void Update()
    {
        if (!mMazeGenerator.MazeGenerationCompleted)
            return;

        if (Input.GetKeyDown("space"))
        {
            //RayCastAndSetDestination();
            SetDestination();


        }
    }

    void OnPathNotFound()
    {
        Debug.Log("Cannot find path to destination");
    }
    void CheakNull()
    {
        if (mStart == null)
        {
            Debug.Log("mStart is null!!");
        }
        else
        {
            Debug.Log("mStart isn't null!!");
        }
        if (mGoal == null)
        {
            Debug.Log("mGoal is null!!");
        }
        else
        {
            Debug.Log("mGoal isn't null!!");
        }
    }


    public void SetDestination()
    {
        Vector2 rayPos = new Vector2(mDestination.transform.position.x, mDestination.transform.position.y);
        RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f);
        if (hit)
        {
            GameObject obj = hit.transform.gameObject;
            MazeCell mazeCell = obj.GetComponent<MazeCell>();
            if (mazeCell == null) return;
            mDestination.gameObject.SetActive(true);

            Vector3 pos = mDestination.position;
            pos.x = mazeCell.transform.position.x;
            pos.y = mazeCell.transform.position.y;
            mDestination.position = pos;
            mStart = mGoal;
            mGoal = mazeCell.Cell;
            //mNpc.AddWayPoint(new Vector2(mDestination.transform.position.x, mDestination.transform.position.y));
            FindPath();
        }
    }
    /*
    void RayCastAndSetDestination()
        {
            // disable picking if we hit the UI.
            if (EventSystem.current.IsPointerOverGameObject() ||
              enabled == false)
            {
                return;
            }
            Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f);
            if (hit)
            {
                GameObject obj = hit.transform.gameObject;
                MazeCell mazeCell = obj.GetComponent<MazeCell>();
                if (mazeCell == null) return;
                Vector3 pos = mDestination.position;
                pos.x = mazeCell.transform.position.x;
                pos.y = mazeCell.transform.position.y;
                mDestination.position = pos;
                mStart = mGoal;
                mGoal = mazeCell.Cell;
                //mNpc.AddWayPoint(new Vector2(obj.transform.position.x, obj.transform.position.y));
                
                // finally show the destination game object.
                mDestination.gameObject.SetActive(true);
            FindPath();
        }
        */





        public void FindPath()
        {
        //CheakNull();
        mPathFinder.Initialize(mStart, mGoal);
            StartCoroutine(Coroutine_FindPathSteps());
            // reset the line.
            mPathViz.positionCount = 0;
        }


        IEnumerator Coroutine_FindPathSteps()
        {
            while (mPathFinder.Status == PathFinderStatus.RUNNING)
            {
                mPathFinder.Step();
                yield return null;
            }
            if (mPathFinder.Status == PathFinderStatus.SUCCESS)
            {
                OnPathFound();
            }
            else if (mPathFinder.Status == PathFinderStatus.FAILURE)
            {
                OnPathNotFound();
            }
        }

       
    }


