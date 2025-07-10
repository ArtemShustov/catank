using UnityEngine;
using System.Collections.Generic;

namespace Game.Utils {
    [ExecuteInEditMode]
    public class WireBuilder: MonoBehaviour {
       [SerializeField] private Transform _start;
       [SerializeField] private Transform _end;
       [SerializeField] private float _density = 1;
       [SerializeField] private float _sagFactor = 0.1f;
       [SerializeField] private LineRenderer _line;
       
       #if UNITY_EDITOR
       private Vector3 _lastStartPos;
       private Vector3 _lastEndPos;
       
       private void Update() {
           if (_start == null || _end == null || _line == null) {
               return;
           }
           
           if (_lastStartPos != _start.position || _lastEndPos != _end.position) { 
               BuildWire();
               _lastStartPos = _start.position;
               _lastEndPos = _end.position;
           }
       }
       private void OnValidate() {
           if (_start == null || _end == null || _line == null) {
               return;
           }
           
           BuildWire();
       }
       #endif
       
       private void BuildWire() {
           Vector3 startLocal = _line.transform.InverseTransformPoint(_start.position);
           Vector3 endLocal = _line.transform.InverseTransformPoint(_end.position);

           float horizontalDistance = Vector3.Distance(new Vector3(startLocal.x, 0, startLocal.z), new Vector3(endLocal.x, 0, endLocal.z));
           float verticalOffset = endLocal.y - startLocal.y;

           float calculatedSag = horizontalDistance * _sagFactor;
           calculatedSag = Mathf.Max(0, calculatedSag);

           float a = CalculateCatenaryParameter(horizontalDistance, calculatedSag, verticalOffset);

           float totalLength = Vector3.Distance(startLocal, endLocal);
           int numPoints = Mathf.Max(2, Mathf.RoundToInt(totalLength * _density));
           _line.positionCount = numPoints;

           List<Vector3> wirePoints = new List<Vector3>();

           Vector3 flatDirection = (new Vector3(endLocal.x, 0, endLocal.z) - new Vector3(startLocal.x, 0, startLocal.z)).normalized;

           for (int i = 0; i < numPoints; i++) {
               float t = (float)i / (numPoints - 1);

               float currentXOffset = horizontalDistance * t;

               float straightLineY = Mathf.Lerp(startLocal.y, endLocal.y, t);

               float interpolatedY = straightLineY - (calculatedSag * (1 - Mathf.Pow(Mathf.Abs(2 * t - 1), 2)));
               
               Vector3 currentPoint = startLocal + flatDirection * currentXOffset;
               currentPoint.y = interpolatedY;

               wirePoints.Add(currentPoint);
           }

           _line.SetPositions(wirePoints.ToArray());
       }
       private float CalculateCatenaryParameter(float horizontalDistance, float desiredSag, float verticalOffset) {
           if (desiredSag <= 0 || horizontalDistance <= 0) {
               return 1000f;
           }

           float tolerance = 0.001f;
           int maxIterations = 100;
           float a = horizontalDistance / 2f;

           for (int i = 0; i < maxIterations; i++) {
               float prevA = a;

               float x_h = horizontalDistance / (2 * a);
               float f = a * Mathf.Cos(x_h) - a - desiredSag;
               float df = Mathf.Cos(x_h) - x_h * Mathf.Sin(x_h) - 1;

               if (Mathf.Abs(df) < float.Epsilon) {
                   break;
               }

               a = prevA - f / df;
               if (a <= 0) {
                   a = prevA / 2;
               }

               if (Mathf.Abs(a - prevA) < tolerance) {
                   break;
               }
           }
           return Mathf.Max(0.01f, a);
       }
    }
}