using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MainContext : MonoBehaviour
{
    [SerializeField] private Transform _camera;
    [SerializeField] private Transform _target;
    [SerializeField] private InputSourceBase _inputSource;
    [SerializeField] private DistanceSetting _distanceSetting;
    [SerializeField] private MoveRecordsStorage _moveRecordsStorage;
    [SerializeField] private RecordSelector _recordSelector;
    [SerializeField] private ButtonTextSwitcher _playStopButton;
    [SerializeField] private ButtonTextSwitcher _recordStopButton;

    private ICameraStrategy _strategy;
    private PersistentRecordsProvider _persistantRecordsProvider;

    private void Start()
    {
        _moveRecordsStorage.ClearRecords();
        _persistantRecordsProvider = new PersistentRecordsProvider(_moveRecordsStorage);
        _persistantRecordsProvider.AddRecordsFromPersistantData();
        _recordSelector.FillDropDownFrom(_moveRecordsStorage);

        _strategy = new CompositeStrategy(
            new ICameraStrategy[] 
                { 
                    new UserControlStrategy(_camera, _target, _inputSource, _distanceSetting)
                });
        _strategy.Init(Time.time);
        _playStopButton.OnClicked += StartStopPlayback;
        _recordStopButton.OnClicked += RecordStop;
    }

    private void StartStopPlayback(bool stop)
    {
        if (stop)
        {
            StopRecord();
        }
        else
        {
            Play();
        }
    }

    private void RecordStop(bool stop)
    {
        if (stop)
        {
            StopRecord();
        }
        else
        {
            StartRecord();
        }
    }

    private void Update()
    {
        _strategy.Update(Time.time);
    }

    private void StartRecord()
    {
        _strategy = new CompositeStrategy(
        new ICameraStrategy[]
            {
                    new UserControlStrategy(_camera, _target, _inputSource, _distanceSetting),
                    new RecordStrategy(_camera, _target, _moveRecordsStorage, _inputSource)
            });
        _strategy.Init(Time.time);
    }

    private void Play()
    {
        _strategy = new CompositeStrategy(
        new ICameraStrategy[]
            {
                    new PlaybackStrategy(_camera, _target, _distanceSetting, _recordSelector.GetRecord())
            });
        _strategy.Init(Time.time);
    }

    private void StopRecord()
    {
        _strategy.Finish();
        _strategy = new CompositeStrategy(
      new ICameraStrategy[]
          {
                    new UserControlStrategy(_camera, _target, _inputSource, _distanceSetting)
          });
        _strategy.Init(Time.time);
        _persistantRecordsProvider.AddRecordsFromPersistantData();
        _recordSelector.FillDropDownFrom(_moveRecordsStorage);
    }

    private void OnDestroy()
    {
        _playStopButton.OnClicked -= StartStopPlayback;
        _recordStopButton.OnClicked -= RecordStop;
    }
}
