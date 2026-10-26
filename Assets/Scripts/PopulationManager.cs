using UnityEngine;
using UnityEngine.InputSystem; 
using System.Collections.Generic;
using System.Linq;

public class PopulationManager : MonoBehaviour
{
    [SerializeField] GameObject personPrefab;
    [SerializeField] int populationSize = 10;
    [SerializeField] float trialTime = 10f;       
    private float elapsed = 0f;         
    [SerializeField] Camera sceneCamera;   
    [SerializeField] private float selectedFraction = 0.5f;       
    [SerializeField] private float mutationRate = 0.02f;

    private List<DNA> population = new List<DNA>(); 

    void Start()
    {
        CreateInitialPopulation();
    }

    void CreateInitialPopulation()
    {
        for (int i = 0; i < populationSize; i++)
        {
            Vector2 randomPos = new Vector2(Random.Range(-8f, 8f), Random.Range(-4f, 4f));
            
            DNA person = Instantiate(personPrefab, randomPos, Quaternion.identity).GetComponent<DNA>();
            person.Initialize(Random.value, Random.value, Random.value, Random.Range(0.5f, 1.5f));
            population.Add(person);
        }
    }

    void Update()
    {
    
        elapsed += Time.deltaTime;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 p = Mouse.current.position.ReadValue();
            Vector3 world = sceneCamera.ScreenToWorldPoint(
                new Vector3(p.x, p.y, -sceneCamera.transform.position.z));
        
            Collider2D hit = Physics2D.OverlapPoint((Vector2)world);
            if (hit != null && hit.TryGetComponent(out DNA target))
            {
                target.Kill(elapsed); 
            }
        }

        
        if (elapsed >= trialTime)
        {
            BreedNewPopulation(); 
            elapsed = 0f;         
        }
    }

    void BreedNewPopulation()
    {
        foreach (DNA d in population)
        {
            d.FinishGeneration(trialTime);
        }
        var ranked = population.OrderBy(_ => Random.value)
            .OrderByDescending(d => d.SurvivalTime).ToList();
        
        int k = Mathf.Clamp(Mathf.CeilToInt(populationSize * selectedFraction), 2, ranked.Count);

        List<DNA> newChildrenList = new List<DNA>();

        for (int i = 0; i < populationSize; i++)
        {
            DNA a = ranked[Random.Range(0, k)];
            DNA b = ranked[Random.Range(0, k)];
            
            DNA child = Breed(a, b); 
            newChildrenList.Add(child);
        }

        foreach (DNA old in population)
        {
            Destroy(old.gameObject);
        }

        population.Clear();
        population = newChildrenList;
        elapsed = 0f;
    }

    private DNA Breed(DNA mother, DNA father)
    {
        float childR = InheritOrMutate(mother.r, father.r);
        float childG = InheritOrMutate(mother.g, father.g);
        float childB = InheritOrMutate(mother.b, father.b);
        float childS = InheritOrMutate(mother.s, father.s);

        Vector2 randomPos = new Vector2(Random.Range(-8f, 8f), Random.Range(-4f, 4f));
        DNA childObj = Instantiate(personPrefab, randomPos, Quaternion.identity).GetComponent<DNA>();
        
        
        childObj.Initialize(childR, childG, childB,childS);

        return childObj;
    }

    private float InheritOrMutate(float motherGene, float fatherGene)
    {
        float inherited = Random.value < 0.5f ? motherGene : fatherGene;
        if (Random.value < mutationRate)
        {
            return Random.value; 
        }
        return inherited;
    }

    
}

