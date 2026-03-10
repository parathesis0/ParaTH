using System.Runtime.CompilerServices;
using ParaTH;
using Xunit;

namespace ParaTH.Tests;

// Simple test components
public struct Position : IEquatable<Position>
{
    public float X;
    public float Y;

    public Position(float x, float y) { X = x; Y = y; }

    public bool Equals(Position other) => X == other.X && Y == other.Y;
    public override bool Equals(object? obj) => obj is Position p && Equals(p);
    public override int GetHashCode() => HashCode.Combine(X, Y);
}

public struct Velocity : IEquatable<Velocity>
{
    public float Dx;
    public float Dy;

    public Velocity(float dx, float dy) { Dx = dx; Dy = dy; }

    public bool Equals(Velocity other) => Dx == other.Dx && Dy == other.Dy;
    public override bool Equals(object? obj) => obj is Velocity v && Equals(v);
    public override int GetHashCode() => HashCode.Combine(Dx, Dy);
}

public struct Health : IEquatable<Health>
{
    public int Hp;

    public Health(int hp) { Hp = hp; }

    public bool Equals(Health other) => Hp == other.Hp;
    public override bool Equals(object? obj) => obj is Health h && Equals(h);
    public override int GetHashCode() => Hp.GetHashCode();
}

public struct Damage : IEquatable<Damage>
{
    public int Value;

    public Damage(int value) { Value = value; }

    public bool Equals(Damage other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is Damage d && Equals(d);
    public override int GetHashCode() => Value.GetHashCode();
}

public class WorldTests
{
    // reasonable defaults: 4096 byte chunks, 64 entities per chunk base, 16 archetype cap, 256 entity cap
    private static World CreateWorld() => new World(4096, 64, 16, 256);

    #region CreateEntity

    [Fact]
    public void CreateEntity_Single_ReturnsValidEntity()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(1, 2));

        Assert.True(world.IsAlive(entity));
    }

    [Fact]
    public void CreateEntity_Single_ComponentValueIsCorrect()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(3, 4));

        ref var pos = ref world.GetComponent<Position>(entity);
        Assert.Equal(3f, pos.X);
        Assert.Equal(4f, pos.Y);
    }

    [Fact]
    public void CreateEntity_Multiple_EachHasCorrectComponents()
    {
        var world = CreateWorld();
        var e1 = world.CreateEntity(new Position(1, 2));
        var e2 = world.CreateEntity(new Position(3, 4));
        var e3 = world.CreateEntity(new Position(5, 6));

        Assert.Equal(1f, world.GetComponent<Position>(e1).X);
        Assert.Equal(3f, world.GetComponent<Position>(e2).X);
        Assert.Equal(5f, world.GetComponent<Position>(e3).X);
    }

    [Fact]
    public void CreateEntity_DifferentComponentTypes_CreatedInDifferentArchetypes()
    {
        var world = CreateWorld();
        var e1 = world.CreateEntity(new Position(1, 2));
        var e2 = world.CreateEntity(new Velocity(3, 4));

        Assert.True(world.HasComponent<Position>(e1));
        Assert.False(world.HasComponent<Velocity>(e1));
        Assert.True(world.HasComponent<Velocity>(e2));
        Assert.False(world.HasComponent<Position>(e2));
    }

    [Fact]
    public void CreateEntity_UniqueIds()
    {
        var world = CreateWorld();
        var e1 = world.CreateEntity(new Position(0, 0));
        var e2 = world.CreateEntity(new Position(0, 0));

        Assert.NotEqual(e1, e2);
        Assert.NotEqual(e1.Id, e2.Id);
    }

    [Fact]
    public void CreateEntity_FirstEntityVersion_IsOne()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(0, 0));

        Assert.Equal(1, entity.Version);
    }

    #endregion

    #region CreateEntity Variadic (T0, T1)

    [Fact]
    public void CreateEntity_TwoComponents_BothAccessible()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(10, 20), new Velocity(1, 2));

        Assert.True(world.HasComponent<Position>(entity));
        Assert.True(world.HasComponent<Velocity>(entity));

        ref var pos = ref world.GetComponent<Position>(entity);
        Assert.Equal(10f, pos.X);
        Assert.Equal(20f, pos.Y);
    }

    #endregion

    #region DestroyEntity

    [Fact]
    public void DestroyEntity_EntityIsNoLongerAlive()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(1, 2));

        world.DestroyEntity(entity);

        Assert.False(world.IsAlive(entity));
    }

    [Fact]
    public void DestroyEntity_ThenCreateNew_ReuseId_VersionIncremented()
    {
        var world = CreateWorld();
        var e1 = world.CreateEntity(new Position(1, 2));
        var oldId = e1.Id;

        world.DestroyEntity(e1);

        var e2 = world.CreateEntity(new Position(3, 4));

        // should reuse the destroyed entity's id with incremented version
        Assert.Equal(oldId, e2.Id);
        Assert.Equal(e1.Version + 1, e2.Version);
    }

    [Fact]
    public void DestroyEntity_StaleHandle_NotAlive()
    {
        var world = CreateWorld();
        var e1 = world.CreateEntity(new Position(1, 2));

        world.DestroyEntity(e1);
        var e2 = world.CreateEntity(new Position(3, 4)); // reuses e1's id

        // old handle should still be considered dead
        Assert.False(world.IsAlive(e1));
        Assert.True(world.IsAlive(e2));
    }

    [Fact]
    public void DestroyEntity_MiddleEntity_OthersStillAccessible()
    {
        var world = CreateWorld();
        var e1 = world.CreateEntity(new Position(1, 0));
        var e2 = world.CreateEntity(new Position(2, 0));
        var e3 = world.CreateEntity(new Position(3, 0));

        world.DestroyEntity(e2);

        Assert.True(world.IsAlive(e1));
        Assert.False(world.IsAlive(e2));
        Assert.True(world.IsAlive(e3));

        // remaining entities should still have correct component values
        Assert.Equal(1f, world.GetComponent<Position>(e1).X);
        Assert.Equal(3f, world.GetComponent<Position>(e3).X);
    }

    [Fact]
    public void DestroyEntity_AllEntities_ThenRecreate()
    {
        var world = CreateWorld();
        var e1 = world.CreateEntity(new Position(1, 0));
        var e2 = world.CreateEntity(new Position(2, 0));

        world.DestroyEntity(e1);
        world.DestroyEntity(e2);

        var e3 = world.CreateEntity(new Position(10, 0));
        var e4 = world.CreateEntity(new Position(20, 0));

        Assert.True(world.IsAlive(e3));
        Assert.True(world.IsAlive(e4));
        Assert.Equal(10f, world.GetComponent<Position>(e3).X);
        Assert.Equal(20f, world.GetComponent<Position>(e4).X);
    }

    [Fact]
    public void DestroyEntity_DestroyFirst_LastEntityCorrect()
    {
        // Tests swap-and-pop: destroying the first entity moves the last into its slot
        var world = CreateWorld();
        var e1 = world.CreateEntity(new Position(1, 0));
        var e2 = world.CreateEntity(new Position(2, 0));

        world.DestroyEntity(e1);

        Assert.True(world.IsAlive(e2));
        Assert.Equal(2f, world.GetComponent<Position>(e2).X);
    }

    #endregion

    #region IsAlive

    [Fact]
    public void IsAlive_NewEntity_True()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(0, 0));

        Assert.True(world.IsAlive(entity));
    }

    [Fact]
    public void IsAlive_DestroyedEntity_False()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(0, 0));
        world.DestroyEntity(entity);

        Assert.False(world.IsAlive(entity));
    }

    [Fact]
    public void IsAlive_FabricatedEntity_False()
    {
        var world = CreateWorld();
        // entity that was never created
        var fake = new Entity(999, 1);

        Assert.False(world.IsAlive(fake));
    }

    #endregion

    #region GetComponent

    [Fact]
    public void GetComponent_ReturnsRef_CanMutate()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(1, 2));

        ref var pos = ref world.GetComponent<Position>(entity);
        pos.X = 100;
        pos.Y = 200;

        ref var pos2 = ref world.GetComponent<Position>(entity);
        Assert.Equal(100f, pos2.X);
        Assert.Equal(200f, pos2.Y);
    }

    #endregion

    #region GetComponent Variadic (T0, T1)

    [Fact]
    public void GetComponent_TwoComponents_ReturnsCorrectValues()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(5, 6), new Velocity(7, 8));

        var (pos, vel) = world.GetComponent<Position, Velocity>(entity);
        Assert.Equal(5f, pos.X);
        Assert.Equal(6f, pos.Y);
        Assert.Equal(7f, vel.Dx);
        Assert.Equal(8f, vel.Dy);
    }

    #endregion

    #region TryGetComponent

    [Fact]
    public void TryGetComponent_Exists_ReturnsTrue()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(5, 10));

        var found = world.TryGetComponent<Position>(entity, out var pos);

        Assert.True(found);
        Assert.Equal(5f, pos.X);
        Assert.Equal(10f, pos.Y);
    }

    [Fact]
    public void TryGetComponent_NotExists_ReturnsFalse()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(5, 10));

        var found = world.TryGetComponent<Velocity>(entity, out _);

        Assert.False(found);
    }

    #endregion

    #region TryGetComponentRef

    [Fact]
    public void TryGetComponentRef_Exists_ReturnsValidRef()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(7, 8));

        ref var pos = ref world.TryGetComponentRef<Position>(entity);

        Assert.False(Unsafe.IsNullRef(ref pos));
        Assert.Equal(7f, pos.X);
    }

    [Fact]
    public void TryGetComponentRef_NotExists_ReturnsNullRef()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(7, 8));

        ref var vel = ref world.TryGetComponentRef<Velocity>(entity);

        Assert.True(Unsafe.IsNullRef(ref vel));
    }

    #endregion

    #region HasComponent

    [Fact]
    public void HasComponent_Exists_True()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(0, 0));

        Assert.True(world.HasComponent<Position>(entity));
    }

    [Fact]
    public void HasComponent_NotExists_False()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(0, 0));

        Assert.False(world.HasComponent<Velocity>(entity));
    }

    #endregion

    #region HasComponent Variadic (T0, T1)

    [Fact]
    public void HasComponent_TwoComponents_BothExist_True()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(0, 0), new Velocity(0, 0));

        Assert.True(world.HasComponent<Position, Velocity>(entity));
    }

    [Fact]
    public void HasComponent_TwoComponents_OnlyOneExists_False()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(0, 0));

        Assert.False(world.HasComponent<Position, Velocity>(entity));
    }

    #endregion

    #region SetComponentValue

    [Fact]
    public void SetComponentValue_OverwritesValue()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(1, 2));

        world.SetComponentValue(entity, new Position(99, 100));

        ref var pos = ref world.GetComponent<Position>(entity);
        Assert.Equal(99f, pos.X);
        Assert.Equal(100f, pos.Y);
    }

    #endregion

    #region SetComponentValue Variadic (T0, T1)

    [Fact]
    public void SetComponentValue_TwoComponents_OverwritesBoth()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(1, 2), new Velocity(3, 4));

        world.SetComponentValue(entity, new Position(10, 20), new Velocity(30, 40));

        ref var pos = ref world.GetComponent<Position>(entity);
        Assert.Equal(10f, pos.X);
        ref var vel = ref world.GetComponent<Velocity>(entity);
        Assert.Equal(30f, vel.Dx);
    }

    #endregion

    #region AddComponent

    [Fact]
    public void AddComponent_AddsNewComponent()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(1, 2));

        Assert.False(world.HasComponent<Velocity>(entity));

        world.AddComponent(entity, new Velocity(5, 6));

        Assert.True(world.HasComponent<Velocity>(entity));
        ref var vel = ref world.GetComponent<Velocity>(entity);
        Assert.Equal(5f, vel.Dx);
        Assert.Equal(6f, vel.Dy);
    }

    [Fact]
    public void AddComponent_PreservesExistingComponents()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(1, 2));

        world.AddComponent(entity, new Velocity(5, 6));

        // original component should still be intact
        Assert.True(world.HasComponent<Position>(entity));
        ref var pos = ref world.GetComponent<Position>(entity);
        Assert.Equal(1f, pos.X);
        Assert.Equal(2f, pos.Y);
    }

    [Fact]
    public void AddComponent_OtherEntitiesUnaffected()
    {
        var world = CreateWorld();
        var e1 = world.CreateEntity(new Position(1, 0));
        var e2 = world.CreateEntity(new Position(2, 0));

        world.AddComponent(e1, new Velocity(10, 0));

        // e2 should still only have Position
        Assert.True(world.HasComponent<Position>(e2));
        Assert.False(world.HasComponent<Velocity>(e2));
        Assert.Equal(2f, world.GetComponent<Position>(e2).X);
    }

    #endregion

    #region AddComponent Variadic (T0, T1)

    [Fact]
    public void AddComponent_TwoComponents_BothAdded()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(1, 2));

        world.AddComponent(entity, new Velocity(3, 4), new Health(100));

        Assert.True(world.HasComponent<Velocity>(entity));
        Assert.True(world.HasComponent<Health>(entity));
        Assert.Equal(3f, world.GetComponent<Velocity>(entity).Dx);
        Assert.Equal(100, world.GetComponent<Health>(entity).Hp);
        // original preserved
        Assert.Equal(1f, world.GetComponent<Position>(entity).X);
    }

    #endregion

    #region RemoveComponent

    [Fact]
    public void RemoveComponent_RemovesTheComponent()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(1, 2), new Velocity(3, 4));

        world.RemoveComponent<Velocity>(entity);

        Assert.False(world.HasComponent<Velocity>(entity));
    }

    [Fact]
    public void RemoveComponent_PreservesOtherComponents()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(1, 2), new Velocity(3, 4));

        world.RemoveComponent<Velocity>(entity);

        Assert.True(world.HasComponent<Position>(entity));
        ref var pos = ref world.GetComponent<Position>(entity);
        Assert.Equal(1f, pos.X);
        Assert.Equal(2f, pos.Y);
    }

    [Fact]
    public void RemoveComponent_OtherEntitiesUnaffected()
    {
        var world = CreateWorld();
        var e1 = world.CreateEntity(new Position(1, 0), new Velocity(10, 0));
        var e2 = world.CreateEntity(new Position(2, 0), new Velocity(20, 0));

        world.RemoveComponent<Velocity>(e1);

        Assert.True(world.HasComponent<Velocity>(e2));
        Assert.Equal(20f, world.GetComponent<Velocity>(e2).Dx);
    }

    #endregion

    #region RemoveComponent Variadic (T0, T1)

    [Fact]
    public void RemoveComponent_TwoComponents_BothRemoved()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(1, 2), new Velocity(3, 4));

        world.AddComponent(entity, new Health(100));

        // now has Position, Velocity, Health
        world.RemoveComponent<Position, Velocity>(entity);

        Assert.False(world.HasComponent<Position>(entity));
        Assert.False(world.HasComponent<Velocity>(entity));
        Assert.True(world.HasComponent<Health>(entity));
        Assert.Equal(100, world.GetComponent<Health>(entity).Hp);
    }

    #endregion

    #region Complex Scenarios

    [Fact]
    public void AddThenRemoveComponent_ReturnsToOriginalArchetype()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(42, 0));

        world.AddComponent(entity, new Velocity(1, 0));
        world.RemoveComponent<Velocity>(entity);

        Assert.True(world.HasComponent<Position>(entity));
        Assert.False(world.HasComponent<Velocity>(entity));
        Assert.Equal(42f, world.GetComponent<Position>(entity).X);
    }

    [Fact]
    public void MultipleArchetypeMigrations_DataIntact()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(1, 2));

        world.AddComponent(entity, new Velocity(3, 4));
        world.AddComponent(entity, new Health(100));

        Assert.Equal(1f, world.GetComponent<Position>(entity).X);
        Assert.Equal(3f, world.GetComponent<Velocity>(entity).Dx);
        Assert.Equal(100, world.GetComponent<Health>(entity).Hp);

        world.RemoveComponent<Velocity>(entity);

        Assert.True(world.HasComponent<Position>(entity));
        Assert.True(world.HasComponent<Health>(entity));
        Assert.False(world.HasComponent<Velocity>(entity));
        Assert.Equal(1f, world.GetComponent<Position>(entity).X);
        Assert.Equal(100, world.GetComponent<Health>(entity).Hp);
    }

    [Fact]
    public void CreateDestroy_ManyEntities_StressTest()
    {
        var world = CreateWorld();
        var entities = new Entity[100];

        for (int i = 0; i < 100; i++)
            entities[i] = world.CreateEntity(new Position(i, i * 10));

        // verify all
        for (int i = 0; i < 100; i++)
        {
            Assert.True(world.IsAlive(entities[i]));
            Assert.Equal((float)i, world.GetComponent<Position>(entities[i]).X);
        }

        // destroy every other entity
        for (int i = 0; i < 100; i += 2)
            world.DestroyEntity(entities[i]);

        // verify remaining
        for (int i = 1; i < 100; i += 2)
        {
            Assert.True(world.IsAlive(entities[i]));
            Assert.Equal((float)i, world.GetComponent<Position>(entities[i]).X);
        }

        // verify destroyed
        for (int i = 0; i < 100; i += 2)
            Assert.False(world.IsAlive(entities[i]));
    }

    [Fact]
    public void DestroyEntity_RecycleAndCreateNew_NewEntityDataCorrect()
    {
        var world = CreateWorld();
        var e1 = world.CreateEntity(new Position(1, 0));
        var e2 = world.CreateEntity(new Position(2, 0));
        var e3 = world.CreateEntity(new Position(3, 0));

        // destroy middle
        world.DestroyEntity(e2);

        // create new, should recycle e2's id
        var e4 = world.CreateEntity(new Position(99, 0));

        Assert.Equal(e2.Id, e4.Id);
        Assert.Equal(99f, world.GetComponent<Position>(e4).X);

        // e1 and e3 should still be valid
        Assert.Equal(1f, world.GetComponent<Position>(e1).X);
        Assert.Equal(3f, world.GetComponent<Position>(e3).X);
    }

    [Fact]
    public void DestroyAndRecreate_MultipleRounds()
    {
        var world = CreateWorld();
        var entity = world.CreateEntity(new Position(1, 0));

        for (int round = 0; round < 5; round++)
        {
            var id = entity.Id;
            world.DestroyEntity(entity);
            Assert.False(world.IsAlive(entity));

            entity = world.CreateEntity(new Position(round * 10, 0));
            Assert.Equal(id, entity.Id); // should reuse same id
            Assert.True(world.IsAlive(entity));
            Assert.Equal(round * 10f, world.GetComponent<Position>(entity).X);
        }
    }

    [Fact]
    public void MixedArchetypes_CreateDestroyAdd_Complex()
    {
        var world = CreateWorld();

        // create entities with different archetype combos
        var e1 = world.CreateEntity(new Position(1, 0));
        var e2 = world.CreateEntity(new Position(2, 0), new Velocity(2, 0));
        var e3 = world.CreateEntity(new Position(3, 0));

        // migrate e1 to have Velocity too
        world.AddComponent(e1, new Velocity(10, 0));

        // destroy e2
        world.DestroyEntity(e2);

        // e1 should still work in the Position+Velocity archetype
        Assert.True(world.IsAlive(e1));
        Assert.Equal(1f, world.GetComponent<Position>(e1).X);
        Assert.Equal(10f, world.GetComponent<Velocity>(e1).Dx);

        // e3 should still be in Position-only archetype
        Assert.True(world.IsAlive(e3));
        Assert.Equal(3f, world.GetComponent<Position>(e3).X);
    }

    #endregion
}
